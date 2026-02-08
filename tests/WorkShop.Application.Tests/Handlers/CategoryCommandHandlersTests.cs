using Moq;
using Xunit;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Commands.Categories;

namespace WorkShop.Application.Tests.Handlers;

public class CategoryCommandHandlersTests
{
    [Fact]
    public async Task CreateCategoryCommandHandler_CreatesCategory()
    {
        // Arrange
        var mockCategoryService = new Mock<ICategoryService>();
        var categoryRequest = new CategoryRequestModel
        {
            Name = "New Category",
            Description = "New Category Description"
        };
        var expectedCategory = new CategoryResponseModel
        {
            Id = 1,
            Name = categoryRequest.Name,
            Description = categoryRequest.Description
        };
        mockCategoryService.Setup(s => s.CreateCategoryAsync(categoryRequest)).ReturnsAsync(expectedCategory);
        
        var handler = new CreateCategoryCommandHandler(mockCategoryService.Object);
        var command = new CreateCategoryCommand(categoryRequest);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Category", result.Name);
        mockCategoryService.Verify(s => s.CreateCategoryAsync(categoryRequest), Times.Once);
    }

    [Fact]
    public async Task UpdateCategoryCommandHandler_UpdatesCategory_WhenCategoryExists()
    {
        // Arrange
        var mockCategoryService = new Mock<ICategoryService>();
        var categoryRequest = new CategoryRequestModel
        {
            Name = "Updated Category",
            Description = "Updated Description"
        };
        var expectedCategory = new CategoryResponseModel
        {
            Id = 1,
            Name = categoryRequest.Name,
            Description = categoryRequest.Description
        };
        mockCategoryService.Setup(s => s.UpdateCategoryAsync(1, categoryRequest)).ReturnsAsync(expectedCategory);
        
        var handler = new UpdateCategoryCommandHandler(mockCategoryService.Object);
        var command = new UpdateCategoryCommand(1, categoryRequest);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Category", result.Name);
        mockCategoryService.Verify(s => s.UpdateCategoryAsync(1, categoryRequest), Times.Once);
    }

    [Fact]
    public async Task UpdateCategoryCommandHandler_ReturnsNull_WhenCategoryDoesNotExist()
    {
        // Arrange
        var mockCategoryService = new Mock<ICategoryService>();
        var categoryRequest = new CategoryRequestModel
        {
            Name = "Updated Category",
            Description = "Updated Description"
        };
        mockCategoryService.Setup(s => s.UpdateCategoryAsync(999, categoryRequest)).ReturnsAsync((CategoryResponseModel?)null);
        
        var handler = new UpdateCategoryCommandHandler(mockCategoryService.Object);
        var command = new UpdateCategoryCommand(999, categoryRequest);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.Null(result);
        mockCategoryService.Verify(s => s.UpdateCategoryAsync(999, categoryRequest), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryCommandHandler_DeletesCategory_WhenCategoryExists()
    {
        // Arrange
        var mockCategoryService = new Mock<ICategoryService>();
        mockCategoryService.Setup(s => s.DeleteCategoryAsync(1)).ReturnsAsync(true);
        
        var handler = new DeleteCategoryCommandHandler(mockCategoryService.Object);
        var command = new DeleteCategoryCommand(1);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.True(result);
        mockCategoryService.Verify(s => s.DeleteCategoryAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryCommandHandler_ReturnsFalse_WhenCategoryDoesNotExist()
    {
        // Arrange
        var mockCategoryService = new Mock<ICategoryService>();
        mockCategoryService.Setup(s => s.DeleteCategoryAsync(999)).ReturnsAsync(false);
        
        var handler = new DeleteCategoryCommandHandler(mockCategoryService.Object);
        var command = new DeleteCategoryCommand(999);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.False(result);
        mockCategoryService.Verify(s => s.DeleteCategoryAsync(999), Times.Once);
    }
}
