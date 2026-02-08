using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using WorkShop.API.Middleware;
using WorkShop.Domain.Exceptions;
using Xunit;

namespace WorkShop.API.Tests;

public class GlobalExceptionHandlerMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionHandlerMiddleware>> _mockLogger;
    private readonly Mock<IHostEnvironment> _mockEnvironment;

    public GlobalExceptionHandlerMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<GlobalExceptionHandlerMiddleware>>();
        _mockEnvironment = new Mock<IHostEnvironment>();
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_CallsNextMiddleware()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var nextCalled = false;
        RequestDelegate next = (HttpContext ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new GlobalExceptionHandlerMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_WhenNotFoundException_Returns404()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        RequestDelegate next = (HttpContext ctx) =>
        {
            throw new BookNotFoundException(1);
        };

        var middleware = new GlobalExceptionHandlerMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(404, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        
        Assert.Equal("Book with ID 1 was not found.", response.GetProperty("error").GetString());
        Assert.Equal(404, response.GetProperty("statusCode").GetInt32());
    }

    [Fact]
    public async Task InvokeAsync_WhenBadRequestException_Returns400()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        RequestDelegate next = (HttpContext ctx) =>
        {
            throw new BadRequestException("Invalid input");
        };

        var middleware = new GlobalExceptionHandlerMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(400, context.Response.StatusCode);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        
        Assert.Equal("Invalid input", response.GetProperty("error").GetString());
        Assert.Equal(400, response.GetProperty("statusCode").GetInt32());
    }

    [Fact]
    public async Task InvokeAsync_WhenUnauthorizedException_Returns401()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        RequestDelegate next = (HttpContext ctx) =>
        {
            throw new UnauthorizedException("Unauthorized access");
        };

        var middleware = new GlobalExceptionHandlerMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(401, context.Response.StatusCode);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        
        Assert.Equal("Unauthorized access", response.GetProperty("error").GetString());
        Assert.Equal(401, response.GetProperty("statusCode").GetInt32());
    }

    [Fact]
    public async Task InvokeAsync_WhenUnhandledException_InProduction_ReturnsGenericMessage()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
        
        RequestDelegate next = (HttpContext ctx) =>
        {
            throw new InvalidOperationException("Internal error details");
        };

        var middleware = new GlobalExceptionHandlerMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(500, context.Response.StatusCode);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        
        Assert.Equal("An internal server error occurred. Please try again later.", response.GetProperty("error").GetString());
        Assert.Equal(500, response.GetProperty("statusCode").GetInt32());
    }

    [Fact]
    public async Task InvokeAsync_WhenUnhandledException_InDevelopment_ReturnsDetailedMessage()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Development");
        
        RequestDelegate next = (HttpContext ctx) =>
        {
            throw new InvalidOperationException("Internal error details");
        };

        var middleware = new GlobalExceptionHandlerMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(500, context.Response.StatusCode);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);
        
        Assert.Equal("Internal error details", response.GetProperty("error").GetString());
        Assert.Equal(500, response.GetProperty("statusCode").GetInt32());
    }

    [Fact]
    public async Task InvokeAsync_WhenException_LogsError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        RequestDelegate next = (HttpContext ctx) =>
        {
            throw new BookNotFoundException(1);
        };

        var middleware = new GlobalExceptionHandlerMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}
