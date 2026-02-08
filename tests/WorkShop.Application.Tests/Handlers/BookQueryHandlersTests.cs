using Moq;
using Xunit;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Queries.Books;

namespace WorkShop.Application.Tests.Handlers;

public class BookQueryHandlersTests
{
    [Fact]
    public async Task GetAllBooksQueryHandler_ReturnsAllBooks()
    {
        // Arrange
        var mockBookService = new Mock<IBookService>();
        var expectedBooks = new List<BookResponseModel>
        {
            new BookResponseModel { Id = 1, Title = "Test Book 1", Author = "Author 1" },
            new BookResponseModel { Id = 2, Title = "Test Book 2", Author = "Author 2" }
        };
        mockBookService.Setup(s => s.GetAllBooksAsync()).ReturnsAsync(expectedBooks);
        
        var handler = new GetAllBooksQueryHandler(mockBookService.Object);
        var query = new GetAllBooksQuery();
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        mockBookService.Verify(s => s.GetAllBooksAsync(), Times.Once);
    }

    [Fact]
    public async Task GetBookByIdQueryHandler_ReturnsBook_WhenBookExists()
    {
        // Arrange
        var mockBookService = new Mock<IBookService>();
        var expectedBook = new BookResponseModel { Id = 1, Title = "Test Book", Author = "Author" };
        mockBookService.Setup(s => s.GetBookByIdAsync(1)).ReturnsAsync(expectedBook);
        
        var handler = new GetBookByIdQueryHandler(mockBookService.Object);
        var query = new GetBookByIdQuery(1);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Book", result.Title);
        mockBookService.Verify(s => s.GetBookByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetBookByIdQueryHandler_ReturnsNull_WhenBookDoesNotExist()
    {
        // Arrange
        var mockBookService = new Mock<IBookService>();
        mockBookService.Setup(s => s.GetBookByIdAsync(999)).ReturnsAsync((BookResponseModel?)null);
        
        var handler = new GetBookByIdQueryHandler(mockBookService.Object);
        var query = new GetBookByIdQuery(999);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.Null(result);
        mockBookService.Verify(s => s.GetBookByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task GetBooksByAuthorQueryHandler_ReturnsFilteredBooks()
    {
        // Arrange
        var mockBookService = new Mock<IBookService>();
        var expectedBooks = new List<BookResponseModel>
        {
            new BookResponseModel { Id = 1, Title = "Book 1", Author = "Test Author" },
            new BookResponseModel { Id = 2, Title = "Book 2", Author = "Test Author" }
        };
        mockBookService.Setup(s => s.GetBooksByAuthorAsync("Test Author")).ReturnsAsync(expectedBooks);
        
        var handler = new GetBooksByAuthorQueryHandler(mockBookService.Object);
        var query = new GetBooksByAuthorQuery("Test Author");
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        mockBookService.Verify(s => s.GetBooksByAuthorAsync("Test Author"), Times.Once);
    }

    [Fact]
    public async Task GetBooksByCategoryQueryHandler_ReturnsFilteredBooks()
    {
        // Arrange
        var mockBookService = new Mock<IBookService>();
        var expectedBooks = new List<BookResponseModel>
        {
            new BookResponseModel { Id = 1, Title = "Book 1", CategoryId = 1, CategoryName = "Fiction" },
            new BookResponseModel { Id = 2, Title = "Book 2", CategoryId = 1, CategoryName = "Fiction" }
        };
        mockBookService.Setup(s => s.GetBooksByCategoryAsync("Fiction")).ReturnsAsync(expectedBooks);
        
        var handler = new GetBooksByCategoryQueryHandler(mockBookService.Object);
        var query = new GetBooksByCategoryQuery("Fiction");
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        mockBookService.Verify(s => s.GetBooksByCategoryAsync("Fiction"), Times.Once);
    }
}
