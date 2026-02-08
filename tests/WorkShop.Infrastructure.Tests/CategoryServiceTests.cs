using Microsoft.EntityFrameworkCore;
using WorkShop.Application.Models.Request;
using WorkShop.Domain.Entities;
using WorkShop.Domain.Exceptions;
using WorkShop.Infrastructure.Data;
using WorkShop.Infrastructure.Services;

namespace WorkShop.Infrastructure.Tests;

public class CategoryServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Categories.AddRange(
            new Category { Id = 1, Name = "Fiction", Description = "Fiction books" },
            new Category { Id = 2, Name = "Non-Fiction", Description = "Non-fiction books" }
        );
        await context.SaveChangesAsync();
        var service = new CategoryService(context);

        // Act
        var result = await service.GetAllCategoriesAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ReturnsCorrectCategory()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var category = new Category { Id = 1, Name = "Science", Description = "Scientific books" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        var service = new CategoryService(context);

        // Act
        var result = await service.GetCategoryByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Science", result.Name);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ThrowsCategoryNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new CategoryService(context);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CategoryNotFoundException>(
            async () => await service.GetCategoryByIdAsync(999));
        Assert.Equal("Category with ID 999 was not found.", exception.Message);
    }

    [Fact]
    public async Task CreateCategoryAsync_AddsNewCategory()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new CategoryService(context);
        var categoryDto = new CategoryRequestModel
        {
            Name = "History",
            Description = "Historical books"
        };

        // Act
        var result = await service.CreateCategoryAsync(categoryDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("History", result.Name);
        var categoriesInDb = await context.Categories.ToListAsync();
        Assert.Single(categoriesInDb);
    }

    [Fact]
    public async Task UpdateCategoryAsync_UpdatesExistingCategory()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var category = new Category { Id = 1, Name = "Old Name", Description = "Old Description" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        var service = new CategoryService(context);

        var updatedDto = new CategoryRequestModel
        {
            Name = "Updated Name",
            Description = "Updated Description"
        };

        // Act
        var result = await service.UpdateCategoryAsync(1, updatedDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal("Updated Description", result.Description);
        Assert.NotNull(result.UpdatedAt);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ThrowsCategoryNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new CategoryService(context);
        var updatedDto = new CategoryRequestModel
        {
            Name = "Updated Name",
            Description = "Updated Description"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CategoryNotFoundException>(
            async () => await service.UpdateCategoryAsync(999, updatedDto));
        Assert.Equal("Category with ID 999 was not found.", exception.Message);
    }

    [Fact]
    public async Task DeleteCategoryAsync_RemovesCategory()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var category = new Category { Id = 1, Name = "Category to Delete", Description = "Will be deleted" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        var service = new CategoryService(context);

        // Act
        var result = await service.DeleteCategoryAsync(1);

        // Assert
        Assert.True(result);
        var categoriesInDb = await context.Categories.ToListAsync();
        Assert.Empty(categoriesInDb);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ThrowsCategoryNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new CategoryService(context);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CategoryNotFoundException>(
            async () => await service.DeleteCategoryAsync(999));
        Assert.Equal("Category with ID 999 was not found.", exception.Message);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ThrowsBadRequestException_WhenCategoryHasBooks()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var category = new Category { Id = 1, Name = "Fiction", Description = "Fiction books" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        
        var book = new Book 
        { 
            Id = 1, 
            Title = "Test Book", 
            Author = "Test Author", 
            CategoryId = 1, 
            Description = "Test Description",
            PublishedDate = DateTime.UtcNow 
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();
        
        var service = new CategoryService(context);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(
            async () => await service.DeleteCategoryAsync(1));
        Assert.Equal("Cannot delete category that is assigned to books.", exception.Message);
    }
}
