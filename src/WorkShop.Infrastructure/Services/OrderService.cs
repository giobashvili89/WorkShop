using Microsoft.EntityFrameworkCore;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Interfaces;
using WorkShop.Domain.Entities;
using WorkShop.Infrastructure.Data;

namespace WorkShop.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
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
                Status = "Pending"
            };

            decimal totalAmount = 0;

            foreach (var itemDto in orderDto.Items)
            {
                var book = await _context.Books.FindAsync(itemDto.BookId);
                if (book == null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                if (book.StockQuantity < itemDto.Quantity)
                {
                    await transaction.RollbackAsync();
                    return null; // Not enough stock
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

                // Update book stock and sold count
                book.StockQuantity -= itemDto.Quantity;
                book.SoldCount += itemDto.Quantity;
            }

            order.TotalAmount = totalAmount;
            order.Status = "Completed";
            order.CompletedDate = DateTime.UtcNow;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

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
            return null;

        return new OrderResponseModel
        {
            Id = order.Id,
            UserId = order.UserId,
            Username = order.User.Username,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            OrderDate = order.OrderDate,
            CompletedDate = order.CompletedDate,
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
            OrderDate = order.OrderDate,
            CompletedDate = order.CompletedDate,
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

    public async Task<IEnumerable<OrderResponseModel>> GetAllOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return orders.Select(order => new OrderResponseModel
        {
            Id = order.Id,
            UserId = order.UserId,
            Username = order.User.Username,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            OrderDate = order.OrderDate,
            CompletedDate = order.CompletedDate,
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
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order == null || order.Status == "Cancelled")
            return false;

        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // Restore book stock
            foreach (var item in order.OrderItems)
            {
                item.Book.StockQuantity += item.Quantity;
                item.Book.SoldCount -= item.Quantity;
            }

            order.Status = "Cancelled";
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
