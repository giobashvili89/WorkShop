using Moq;
using Xunit;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Request;
using WorkShop.Domain.Entities;
using WorkShop.Domain.Enums;
using WorkShop.Domain.Exceptions;
using WorkShop.Infrastructure.Data;
using WorkShop.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace WorkShop.Infrastructure.Tests;

public class OrderServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidData_ShouldCreateOrder()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockEmailService = new Mock<IEmailService>();
        var orderService = new OrderService(context, mockEmailService.Object);

        var user = new User { Username = "testuser", Email = "test@test.com", PasswordHash = "passwordhash", Role = UserRole.Customer };
        context.Users.Add(user);

        var book = new Book { Title = "Test Book", Author = "Test Author", Price = 10.00m, StockQuantity = 5, SoldCount = 0 };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var orderRequest = new OrderRequestModel
        {
            Items = new List<OrderItemRequestModel>
            {
                new() { BookId = book.Id, Quantity = 2 }
            },
            PhoneNumber = "+1234567890",
            AlternativePhoneNumber = "+0987654321",
            HomeAddress = "123 Test Street"
        };

        // Act
        var result = await orderService.CreateOrderAsync(user.Id, orderRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(20.00m, result.TotalAmount);
        Assert.Equal(OrderStatus.Completed, result.Status);
        Assert.Equal(TrackingStatus.OrderPlaced, result.TrackingStatus);
        Assert.Single(result.Items);
        Assert.Equal(2, result.Items[0].Quantity);

        // Verify stock was updated
        var updatedBook = await context.Books.FindAsync(book.Id);
        Assert.Equal(3, updatedBook?.StockQuantity);
        Assert.Equal(2, updatedBook?.SoldCount);
    }

    [Fact]
    public async Task CreateOrderAsync_WithInsufficientStock_ThrowsBadRequestException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockEmailService = new Mock<IEmailService>();
        var orderService = new OrderService(context, mockEmailService.Object);

        var user = new User { Username = "testuser", Email = "test@test.com", PasswordHash = "passwordhash", Role = UserRole.Customer };
        context.Users.Add(user);

        var book = new Book { Title = "Test Book", Author = "Test Author", Price = 10.00m, StockQuantity = 1, SoldCount = 0 };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var orderRequest = new OrderRequestModel
        {
            Items = new List<OrderItemRequestModel>
            {
                new() { BookId = book.Id, Quantity = 5 }
            },
            PhoneNumber = "+1234567890",
            HomeAddress = "123 Test Street"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(
            async () => await orderService.CreateOrderAsync(user.Id, orderRequest));
        Assert.Contains("Insufficient stock", exception.Message);
        Assert.Contains("Test Book", exception.Message);

        // Verify stock was not changed
        var unchangedBook = await context.Books.FindAsync(book.Id);
        Assert.Equal(1, unchangedBook?.StockQuantity);
        Assert.Equal(0, unchangedBook?.SoldCount);
    }

    [Fact]
    public async Task CreateOrderAsync_WithNonExistentBook_ThrowsBookNotFoundException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockEmailService = new Mock<IEmailService>();
        var orderService = new OrderService(context, mockEmailService.Object);

        var user = new User { Username = "testuser", Email = "test@test.com", PasswordHash = "passwordhash", Role = UserRole.Customer };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var orderRequest = new OrderRequestModel
        {
            Items = new List<OrderItemRequestModel>
            {
                new() { BookId = 999, Quantity = 1 }
            },
            PhoneNumber = "+1234567890",
            HomeAddress = "123 Test Street"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BookNotFoundException>(
            async () => await orderService.CreateOrderAsync(user.Id, orderRequest));
        Assert.Equal("Book with ID 999 was not found.", exception.Message);
    }

    [Fact]
    public async Task GetAllOrdersAsync_WithoutFilters_ShouldReturnAllOrders()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockEmailService = new Mock<IEmailService>();
        var orderService = new OrderService(context, mockEmailService.Object);

        var user1 = new User { Username = "user1", Email = "user1@test.com", PasswordHash = "passwordhash", Role = UserRole.Customer };
        var user2 = new User { Username = "user2", Email = "user2@test.com", PasswordHash = "passwordhash", Role = UserRole.Customer };
        context.Users.AddRange(user1, user2);

        var book = new Book { Title = "Test Book", Author = "Test Author", Price = 10.00m, StockQuantity = 10 };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var order1 = new Order
        {
            UserId = user1.Id,
            TotalAmount = 20m,
            Status = OrderStatus.Completed,
            TrackingStatus = TrackingStatus.Delivered,
            PhoneNumber = "+1234567890",
            HomeAddress = "123 Test St",
            OrderItems = new List<OrderItem> { new() { BookId = book.Id, Quantity = 2, UnitPrice = 10m, TotalPrice = 20m } }
        };
        var order2 = new Order
        {
            UserId = user2.Id,
            TotalAmount = 30m,
            Status = OrderStatus.Pending,
            TrackingStatus = TrackingStatus.Processing,
            PhoneNumber = "+0987654321",
            HomeAddress = "456 Test Ave",
            OrderItems = new List<OrderItem> { new() { BookId = book.Id, Quantity = 3, UnitPrice = 10m, TotalPrice = 30m } }
        };
        context.Orders.AddRange(order1, order2);
        await context.SaveChangesAsync();

        // Act
        var result = await orderService.GetAllOrdersAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllOrdersAsync_WithStatusFilter_ShouldReturnFilteredOrders()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockEmailService = new Mock<IEmailService>();
        var orderService = new OrderService(context, mockEmailService.Object);

        var user = new User { Username = "user1", Email = "user1@test.com", PasswordHash = "passwordhash", Role = UserRole.Customer };
        context.Users.Add(user);

        var book = new Book { Title = "Test Book", Author = "Test Author", Price = 10.00m, StockQuantity = 10 };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var order1 = new Order
        {
            UserId = user.Id,
            TotalAmount = 20m,
            Status = OrderStatus.Completed,
            TrackingStatus = TrackingStatus.Delivered,
            PhoneNumber = "+1234567890",
            HomeAddress = "123 Test St",
            OrderItems = new List<OrderItem> { new() { BookId = book.Id, Quantity = 2, UnitPrice = 10m, TotalPrice = 20m } }
        };
        var order2 = new Order
        {
            UserId = user.Id,
            TotalAmount = 30m,
            Status = OrderStatus.Pending,
            TrackingStatus = TrackingStatus.Processing,
            PhoneNumber = "+0987654321",
            HomeAddress = "456 Test Ave",
            OrderItems = new List<OrderItem> { new() { BookId = book.Id, Quantity = 3, UnitPrice = 10m, TotalPrice = 30m } }
        };
        context.Orders.AddRange(order1, order2);
        await context.SaveChangesAsync();

        // Act
        var result = await orderService.GetAllOrdersAsync(status: "Completed");

        // Assert
        Assert.Single(result);
        Assert.Equal(OrderStatus.Completed, result.First().Status);
    }

    [Fact]
    public async Task CancelOrderAsync_WithinOneHour_ShouldCancelAndRestoreStock()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockEmailService = new Mock<IEmailService>();
        var orderService = new OrderService(context, mockEmailService.Object);

        var user = new User { Username = "testuser", Email = "test@test.com", PasswordHash = "passwordhash", Role = UserRole.Customer };
        context.Users.Add(user);

        var book = new Book { Title = "Test Book", Author = "Test Author", Price = 10.00m, StockQuantity = 3, SoldCount = 2 };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var order = new Order
        {
            UserId = user.Id,
            TotalAmount = 20m,
            Status = OrderStatus.Completed,
            OrderDate = DateTime.UtcNow.AddMinutes(-30), // 30 minutes ago
            PhoneNumber = "+1234567890",
            HomeAddress = "123 Test St",
            OrderItems = new List<OrderItem>
            {
                new() { BookId = book.Id, Quantity = 2, UnitPrice = 10m, TotalPrice = 20m }
            }
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        // Act
        var result = await orderService.CancelOrderAsync(order.Id, user.Id);

        // Assert
        Assert.True(result);
        var cancelledOrder = await context.Orders.FindAsync(order.Id);
        Assert.Equal(OrderStatus.Cancelled, cancelledOrder?.Status);

        // Verify stock was restored
        var restoredBook = await context.Books.FindAsync(book.Id);
        Assert.Equal(5, restoredBook?.StockQuantity);
        Assert.Equal(0, restoredBook?.SoldCount);
    }

    [Fact]
    public async Task CancelOrderAsync_AfterOneHour_ThrowsBadRequestException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockEmailService = new Mock<IEmailService>();
        var orderService = new OrderService(context, mockEmailService.Object);

        var user = new User { Username = "testuser", Email = "test@test.com", PasswordHash = "passwordhash", Role = UserRole.Customer };
        context.Users.Add(user);

        var book = new Book { Title = "Test Book", Author = "Test Author", Price = 10.00m, StockQuantity = 3 };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var order = new Order
        {
            UserId = user.Id,
            TotalAmount = 20m,
            Status = OrderStatus.Completed,
            OrderDate = DateTime.UtcNow.AddHours(-2), // 2 hours ago
            PhoneNumber = "+1234567890",
            HomeAddress = "123 Test St",
            OrderItems = new List<OrderItem>
            {
                new() { BookId = book.Id, Quantity = 2, UnitPrice = 10m, TotalPrice = 20m }
            }
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(
            async () => await orderService.CancelOrderAsync(order.Id, user.Id));
        Assert.Equal("Order can only be cancelled within 1 hour of placement.", exception.Message);
    }

    [Fact]
    public async Task CancelOrderAsync_WithNonExistentOrder_ThrowsOrderNotFoundException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockEmailService = new Mock<IEmailService>();
        var orderService = new OrderService(context, mockEmailService.Object);

        var user = new User { Username = "testuser", Email = "test@test.com", PasswordHash = "passwordhash", Role = UserRole.Customer };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OrderNotFoundException>(
            async () => await orderService.CancelOrderAsync(999, user.Id));
        Assert.Equal("Order with ID 999 was not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateDeliveryInfoAsync_ShouldUpdateOrderDetails()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockEmailService = new Mock<IEmailService>();
        var orderService = new OrderService(context, mockEmailService.Object);

        var user = new User { Username = "testuser", Email = "test@test.com", PasswordHash = "passwordhash", Role = UserRole.Customer };
        context.Users.Add(user);

        var book = new Book { Title = "Test Book", Author = "Test Author", Price = 10.00m, StockQuantity = 5 };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var order = new Order
        {
            UserId = user.Id,
            TotalAmount = 20m,
            Status = OrderStatus.Pending,
            TrackingStatus = TrackingStatus.OrderPlaced,
            PhoneNumber = "+1234567890",
            HomeAddress = "123 Test St",
            OrderItems = new List<OrderItem>
            {
                new() { BookId = book.Id, Quantity = 2, UnitPrice = 10m, TotalPrice = 20m }
            }
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var updateModel = new UpdateDeliveryInfoRequestModel
        {
            TrackingStatus = TrackingStatus.InWarehouse,
            Status = OrderStatus.Completed,
            PhoneNumber = "+9999999999",
            SendEmail = false
        };

        // Act
        var result = await orderService.UpdateDeliveryInfoAsync(order.Id, updateModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TrackingStatus.InWarehouse, result.TrackingStatus);
        Assert.Equal(OrderStatus.Completed, result.Status);
        Assert.Equal("+9999999999", result.PhoneNumber);

        var updatedOrder = await context.Orders.FindAsync(order.Id);
        Assert.Equal(TrackingStatus.InWarehouse, updatedOrder?.TrackingStatus);
        Assert.Equal(OrderStatus.Completed, updatedOrder?.Status);
    }

    [Fact]
    public async Task UpdateDeliveryInfoAsync_ThrowsOrderNotFoundException_WhenOrderDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockEmailService = new Mock<IEmailService>();
        var orderService = new OrderService(context, mockEmailService.Object);

        var updateModel = new UpdateDeliveryInfoRequestModel
        {
            TrackingStatus = TrackingStatus.InWarehouse,
            Status = OrderStatus.Completed,
            SendEmail = false
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OrderNotFoundException>(
            async () => await orderService.UpdateDeliveryInfoAsync(999, updateModel));
        Assert.Equal("Order with ID 999 was not found.", exception.Message);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ThrowsOrderNotFoundException_WhenOrderDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockEmailService = new Mock<IEmailService>();
        var orderService = new OrderService(context, mockEmailService.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OrderNotFoundException>(
            async () => await orderService.GetOrderByIdAsync(999));
        Assert.Equal("Order with ID 999 was not found.", exception.Message);
    }
}
