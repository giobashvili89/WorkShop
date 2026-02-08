using Moq;
using Xunit;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Commands.Books;

namespace WorkShop.Application.Tests.Handlers;

public class BookCommandHandlersTests
{
    [Fact]
    public async Task CreateBookCommandHandler_CreatesBook()
    {
        // Arrange
        var mockBookService = new Mock<IBookService>();
        var bookRequest = new BookRequestModel
        {
            Title = "New Book",
            Author = "Author",
            CategoryId = 1,
            Description = "Description",
            ISBN = "1234567890",
            Price = 29.99m,
            StockQuantity = 10,
            PublishedDate = DateTime.UtcNow
        };
        var expectedBook = new BookResponseModel
        {
            Id = 1,
            Title = bookRequest.Title,
            Author = bookRequest.Author
        };
        mockBookService.Setup(s => s.CreateBookAsync(bookRequest)).ReturnsAsync(expectedBook);
        
        var handler = new CreateBookCommandHandler(mockBookService.Object);
        var command = new CreateBookCommand(bookRequest);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Book", result.Title);
        mockBookService.Verify(s => s.CreateBookAsync(bookRequest), Times.Once);
    }

    [Fact]
    public async Task UpdateBookCommandHandler_UpdatesBook_WhenBookExists()
    {
        // Arrange
        var mockBookService = new Mock<IBookService>();
        var bookRequest = new BookRequestModel
        {
            Title = "Updated Book",
            Author = "Author",
            CategoryId = 1,
            Description = "Description",
            ISBN = "1234567890",
            Price = 29.99m,
            StockQuantity = 10,
            PublishedDate = DateTime.UtcNow
        };
        var expectedBook = new BookResponseModel
        {
            Id = 1,
            Title = bookRequest.Title,
            Author = bookRequest.Author
        };
        mockBookService.Setup(s => s.UpdateBookAsync(1, bookRequest)).ReturnsAsync(expectedBook);
        
        var handler = new UpdateBookCommandHandler(mockBookService.Object);
        var command = new UpdateBookCommand(1, bookRequest);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Book", result.Title);
        mockBookService.Verify(s => s.UpdateBookAsync(1, bookRequest), Times.Once);
    }

    [Fact]
    public async Task UpdateBookCommandHandler_ReturnsNull_WhenBookDoesNotExist()
    {
        // Arrange
        var mockBookService = new Mock<IBookService>();
        var bookRequest = new BookRequestModel
        {
            Title = "Updated Book",
            Author = "Author",
            CategoryId = 1,
            Description = "Description",
            ISBN = "1234567890",
            Price = 29.99m,
            StockQuantity = 10,
            PublishedDate = DateTime.UtcNow
        };
        mockBookService.Setup(s => s.UpdateBookAsync(999, bookRequest)).ReturnsAsync((BookResponseModel?)null);
        
        var handler = new UpdateBookCommandHandler(mockBookService.Object);
        var command = new UpdateBookCommand(999, bookRequest);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.Null(result);
        mockBookService.Verify(s => s.UpdateBookAsync(999, bookRequest), Times.Once);
    }

    [Fact]
    public async Task DeleteBookCommandHandler_DeletesBook_WhenBookExists()
    {
        // Arrange
        var mockBookService = new Mock<IBookService>();
        mockBookService.Setup(s => s.DeleteBookAsync(1)).ReturnsAsync(true);
        
        var handler = new DeleteBookCommandHandler(mockBookService.Object);
        var command = new DeleteBookCommand(1);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.True(result);
        mockBookService.Verify(s => s.DeleteBookAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteBookCommandHandler_ReturnsFalse_WhenBookDoesNotExist()
    {
        // Arrange
        var mockBookService = new Mock<IBookService>();
        mockBookService.Setup(s => s.DeleteBookAsync(999)).ReturnsAsync(false);
        
        var handler = new DeleteBookCommandHandler(mockBookService.Object);
        var command = new DeleteBookCommand(999);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.False(result);
        mockBookService.Verify(s => s.DeleteBookAsync(999), Times.Once);
    }
}
