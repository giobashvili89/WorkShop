using Microsoft.EntityFrameworkCore;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Domain.Entities;
using WorkShop.Domain.Exceptions;
using WorkShop.Infrastructure.Data;
using WorkShop.Infrastructure.Services;

namespace WorkShop.Infrastructure.Tests;

public class BookServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new AppDbContext(options);
        
        // Seed test categories
        context.Categories.AddRange(
            new Category { Id = 1, Name = "Fiction", Description = "Fiction books" },
            new Category { Id = 2, Name = "Non-Fiction", Description = "Non-fiction books" }
        );
        context.SaveChanges();
        
        return context;
    }

    [Fact]
    public async Task GetAllBooksAsync_ReturnsAllBooks()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Books.AddRange(
            new Book { Id = 1, Title = "Book 1", Author = "Author 1", CategoryId = 1, Description = "Desc 1", PublishedDate = DateTime.UtcNow },
            new Book { Id = 2, Title = "Book 2", Author = "Author 2", CategoryId = 2, Description = "Desc 2", PublishedDate = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        var service = new BookService(context);

        // Act
        var result = await service.GetAllBooksAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetBookByIdAsync_ReturnsCorrectBook()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var book = new Book { Id = 1, Title = "Test Book", Author = "Test Author", CategoryId = 1, Description = "Test Desc", PublishedDate = DateTime.UtcNow };
        context.Books.Add(book);
        await context.SaveChangesAsync();
        var service = new BookService(context);

        // Act
        var result = await service.GetBookByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Book", result.Title);
        Assert.Equal("Test Author", result.Author);
    }

    [Fact]
    public async Task GetBookByIdAsync_ThrowsBookNotFoundException_WhenBookNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookService(context);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BookNotFoundException>(
            async () => await service.GetBookByIdAsync(999));
        Assert.Equal("Book with ID 999 was not found.", exception.Message);
    }

    [Fact]
    public async Task CreateBookAsync_AddsNewBook()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookService(context);
        var bookDto = new BookRequestModel
        {
            Title = "New Book",
            Author = "New Author",
            CategoryId = 1,
            Description = "New Description",
            PublishedDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var result = await service.CreateBookAsync(bookDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Book", result.Title);
        var booksInDb = await context.Books.ToListAsync();
        Assert.Single(booksInDb);
    }

    [Fact]
    public async Task UpdateBookAsync_UpdatesExistingBook()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var book = new Book { Id = 1, Title = "Old Title", Author = "Old Author", CategoryId = 1, Description = "Old Desc", PublishedDate = DateTime.UtcNow };
        context.Books.Add(book);
        await context.SaveChangesAsync();
        var service = new BookService(context);

        var updatedDto = new BookRequestModel
        {
            Title = "Updated Title",
            Author = "Updated Author",
            CategoryId = 2,
            Description = "Updated Desc",
            PublishedDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var result = await service.UpdateBookAsync(1, updatedDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Updated Author", result.Author);
    }

    [Fact]
    public async Task UpdateBookAsync_ThrowsBookNotFoundException_WhenBookNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookService(context);

        var updatedDto = new BookRequestModel
        {
            Title = "Updated Title",
            Author = "Updated Author",
            CategoryId = 1,
            Description = "Updated Desc",
            PublishedDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BookNotFoundException>(
            async () => await service.UpdateBookAsync(999, updatedDto));
        Assert.Equal("Book with ID 999 was not found.", exception.Message);
    }

    [Fact]
    public async Task DeleteBookAsync_RemovesBook()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var book = new Book { Id = 1, Title = "Book to Delete", Author = "Author", CategoryId = 1, Description = "Desc", PublishedDate = DateTime.UtcNow };
        context.Books.Add(book);
        await context.SaveChangesAsync();
        var service = new BookService(context);

        // Act
        var result = await service.DeleteBookAsync(1);

        // Assert
        Assert.True(result);
        var booksInDb = await context.Books.ToListAsync();
        Assert.Empty(booksInDb);
    }

    [Fact]
    public async Task DeleteBookAsync_ThrowsBookNotFoundException_WhenBookNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookService(context);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BookNotFoundException>(
            async () => await service.DeleteBookAsync(999));
        Assert.Equal("Book with ID 999 was not found.", exception.Message);
    }

    [Fact]
    public async Task GetBooksByAuthorAsync_ReturnsCorrectBooks()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Books.AddRange(
            new Book { Id = 1, Title = "Book 1", Author = "John Doe", CategoryId = 1, Description = "Desc 1", PublishedDate = DateTime.UtcNow },
            new Book { Id = 2, Title = "Book 2", Author = "Jane Smith", CategoryId = 2, Description = "Desc 2", PublishedDate = DateTime.UtcNow },
            new Book { Id = 3, Title = "Book 3", Author = "John Doe", CategoryId = 1, Description = "Desc 3", PublishedDate = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        var service = new BookService(context);

        // Act
        var result = await service.GetBooksByAuthorAsync("John");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Contains("John", b.Author));
    }

    [Fact]
    public async Task GetBooksByCategoryAsync_ReturnsCorrectBooks()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        // Add specific categories for this test
        context.Categories.AddRange(
            new Category { Id = 3, Name = "Science", Description = "Science books" },
            new Category { Id = 4, Name = "History", Description = "History books" }
        );
        context.SaveChanges();
        
        context.Books.AddRange(
            new Book { Id = 1, Title = "Book 1", Author = "Author 1", CategoryId = 3, Description = "Desc 1", PublishedDate = DateTime.UtcNow },
            new Book { Id = 2, Title = "Book 2", Author = "Author 2", CategoryId = 4, Description = "Desc 2", PublishedDate = DateTime.UtcNow },
            new Book { Id = 3, Title = "Book 3", Author = "Author 3", CategoryId = 3, Description = "Desc 3", PublishedDate = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        var service = new BookService(context);

        // Act
        var result = await service.GetBooksByCategoryAsync("Science");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Contains("Science", b.CategoryName));
    }
}
