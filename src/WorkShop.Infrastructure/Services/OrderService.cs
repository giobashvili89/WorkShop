using Microsoft.EntityFrameworkCore;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Interfaces;
using WorkShop.Domain.Entities;
using WorkShop.Domain.Enums;
using WorkShop.Domain.Exceptions;
using WorkShop.Infrastructure.Data;

namespace WorkShop.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;

    public OrderService(AppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<OrderResponseModel?> CreateOrderAsync(int userId, OrderRequestModel orderDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TrackingStatus = TrackingStatus.OrderPlaced,
                PhoneNumber = orderDto.PhoneNumber,
                AlternativePhoneNumber = orderDto.AlternativePhoneNumber,
                HomeAddress = orderDto.HomeAddress
            };

            decimal totalAmount = 0;

            foreach (var itemDto in orderDto.Items)
            {
                var book = await _context.Books.FindAsync(itemDto.BookId);
                if (book == null)
                {
                    await transaction.RollbackAsync();
                    throw new BookNotFoundException(itemDto.BookId);
                }

                if (book.StockQuantity < itemDto.Quantity)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequestException($"Insufficient stock for book '{book.Title}'. Available: {book.StockQuantity}, Requested: {itemDto.Quantity}");
                }

                var orderItem = new OrderItem
                {
                    BookId = itemDto.BookId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = book.Price,
                    TotalPrice = book.Price * itemDto.Quantity
                };

                order.OrderItems.Add(orderItem);
                totalAmount += orderItem.TotalPrice;

                book.StockQuantity -= itemDto.Quantity;
                book.SoldCount += itemDto.Quantity;
            }

            order.TotalAmount = totalAmount;
            order.Status = OrderStatus.Completed;
            order.CompletedDate = DateTime.UtcNow;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                await _emailService.SendOrderConfirmationEmailAsync(order, user);
                order.EmailSent = true;
                await _context.SaveChangesAsync();
            }

            return await GetOrderByIdAsync(order.Id);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<OrderResponseModel?> GetOrderByIdAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            throw new OrderNotFoundException(orderId);

        var canCancel = order.Status != OrderStatus.Cancelled && 
                        (DateTime.UtcNow - order.OrderDate).TotalHours <= 1;

        return new OrderResponseModel
        {
            Id = order.Id,
            UserId = order.UserId,
            Username = order.User.Username,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            TrackingStatus = order.TrackingStatus,
            OrderDate = order.OrderDate,
            CompletedDate = order.CompletedDate,
            CanCancel = canCancel,
            PhoneNumber = order.PhoneNumber,
            AlternativePhoneNumber = order.AlternativePhoneNumber,
            HomeAddress = order.HomeAddress,
            Items = order.OrderItems.Select(oi => new OrderItemResponseModel
            {
                Id = oi.Id,
                BookId = oi.BookId,
                BookTitle = oi.Book.Title,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                TotalPrice = oi.TotalPrice
            }).ToList()
        };
    }

    public async Task<IEnumerable<OrderResponseModel>> GetUserOrdersAsync(int userId)
    {
        var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return orders.Select(order => new OrderResponseModel
        {
            Id = order.Id,
            UserId = order.UserId,
            Username = order.User.Username,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            TrackingStatus = order.TrackingStatus,
            OrderDate = order.OrderDate,
            CompletedDate = order.CompletedDate,
            CanCancel = order.Status != OrderStatus.Cancelled && (DateTime.UtcNow - order.OrderDate).TotalHours <= 1,
            PhoneNumber = order.PhoneNumber,
            AlternativePhoneNumber = order.AlternativePhoneNumber,
            HomeAddress = order.HomeAddress,
            Items = order.OrderItems.Select(oi => new OrderItemResponseModel
            {
                Id = oi.Id,
                BookId = oi.BookId,
                BookTitle = oi.Book.Title,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                TotalPrice = oi.TotalPrice
            }).ToList()
        });
    }

    public async Task<IEnumerable<OrderResponseModel>> GetAllOrdersAsync(string? status = null, 
        string? trackingStatus = null, DateTime? startDate = null, DateTime? endDate = null, 
        string? customerSearch = null, int? orderId = null, decimal? minAmount = null, decimal? maxAmount = null)
    {
        var query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            query = query.Where(o => o.Status == orderStatus);

        if (!string.IsNullOrEmpty(trackingStatus) && Enum.TryParse<TrackingStatus>(trackingStatus, true, out var trackStatus))
            query = query.Where(o => o.TrackingStatus == trackStatus);

        if (startDate.HasValue)
            query = query.Where(o => o.OrderDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(o => o.OrderDate <= endDate.Value);

        if (!string.IsNullOrEmpty(customerSearch))
            query = query.Where(o => o.User.Username.Contains(customerSearch) || 
                                    o.UserId.ToString() == customerSearch);

        if (orderId.HasValue)
            query = query.Where(o => o.Id == orderId.Value);

        if (minAmount.HasValue)
            query = query.Where(o => o.TotalAmount >= minAmount.Value);

        if (maxAmount.HasValue)
            query = query.Where(o => o.TotalAmount <= maxAmount.Value);

        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return orders.Select(order => new OrderResponseModel
        {
            Id = order.Id,
            UserId = order.UserId,
            Username = order.User.Username,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            TrackingStatus = order.TrackingStatus,
            OrderDate = order.OrderDate,
            CompletedDate = order.CompletedDate,
            CanCancel = order.Status != OrderStatus.Cancelled && (DateTime.UtcNow - order.OrderDate).TotalHours <= 1,
            PhoneNumber = order.PhoneNumber,
            AlternativePhoneNumber = order.AlternativePhoneNumber,
            HomeAddress = order.HomeAddress,
            Items = order.OrderItems.Select(oi => new OrderItemResponseModel
            {
                Id = oi.Id,
                BookId = oi.BookId,
                BookTitle = oi.Book.Title,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                TotalPrice = oi.TotalPrice
            }).ToList()
        });
    }

    public async Task<bool> CancelOrderAsync(int orderId, int userId)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order == null || order.Status == OrderStatus.Cancelled)
            throw new OrderNotFoundException(orderId);

        var hoursSinceOrder = (DateTime.UtcNow - order.OrderDate).TotalHours;
        if (hoursSinceOrder > 1)
        {
            throw new BadRequestException("Order can only be cancelled within 1 hour of placement.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            foreach (var item in order.OrderItems)
            {
                item.Book.StockQuantity += item.Quantity;
                item.Book.SoldCount = Math.Max(0, item.Book.SoldCount - item.Quantity);
            }

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            await _emailService.SendOrderCancellationEmailAsync(order, order.User);

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<OrderResponseModel?> UpdateDeliveryInfoAsync(int orderId, UpdateDeliveryInfoRequestModel model)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            throw new OrderNotFoundException(orderId);

        if (model.TrackingStatus.HasValue)
            order.TrackingStatus = model.TrackingStatus.Value;

        if (model.Status.HasValue)
            order.Status = model.Status.Value;

        if (!string.IsNullOrEmpty(model.PhoneNumber))
            order.PhoneNumber = model.PhoneNumber;

        if (!string.IsNullOrEmpty(model.AlternativePhoneNumber))
            order.AlternativePhoneNumber = model.AlternativePhoneNumber;

        if (!string.IsNullOrEmpty(model.HomeAddress))
            order.HomeAddress = model.HomeAddress;

        await _context.SaveChangesAsync();

        if (model.SendEmail)
        {
            await _emailService.SendOrderStatusUpdateEmailAsync(order, order.User);
            order.EmailSent = true;
            await _context.SaveChangesAsync();
        }

        return await GetOrderByIdAsync(orderId);
    }
}

