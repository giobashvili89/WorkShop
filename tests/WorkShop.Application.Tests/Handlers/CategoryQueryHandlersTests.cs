using Moq;
using Xunit;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Queries.Categories;

namespace WorkShop.Application.Tests.Handlers;

public class CategoryQueryHandlersTests
{
    [Fact]
    public async Task GetAllCategoriesQueryHandler_ReturnsAllCategories()
    {
        // Arrange
        var mockCategoryService = new Mock<ICategoryService>();
        var expectedCategories = new List<CategoryResponseModel>
        {
            new CategoryResponseModel { Id = 1, Name = "Fiction", Description = "Fiction books" },
            new CategoryResponseModel { Id = 2, Name = "Non-Fiction", Description = "Non-fiction books" }
        };
        mockCategoryService.Setup(s => s.GetAllCategoriesAsync()).ReturnsAsync(expectedCategories);
        
        var handler = new GetAllCategoriesQueryHandler(mockCategoryService.Object);
        var query = new GetAllCategoriesQuery();
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        mockCategoryService.Verify(s => s.GetAllCategoriesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetCategoryByIdQueryHandler_ReturnsCategory_WhenCategoryExists()
    {
        // Arrange
        var mockCategoryService = new Mock<ICategoryService>();
        var expectedCategory = new CategoryResponseModel
        {
            Id = 1,
            Name = "Fiction",
            Description = "Fiction books"
        };
        mockCategoryService.Setup(s => s.GetCategoryByIdAsync(1)).ReturnsAsync(expectedCategory);
        
        var handler = new GetCategoryByIdQueryHandler(mockCategoryService.Object);
        var query = new GetCategoryByIdQuery(1);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Fiction", result.Name);
        mockCategoryService.Verify(s => s.GetCategoryByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetCategoryByIdQueryHandler_ReturnsNull_WhenCategoryDoesNotExist()
    {
        // Arrange
        var mockCategoryService = new Mock<ICategoryService>();
        mockCategoryService.Setup(s => s.GetCategoryByIdAsync(999)).ReturnsAsync((CategoryResponseModel?)null);
        
        var handler = new GetCategoryByIdQueryHandler(mockCategoryService.Object);
        var query = new GetCategoryByIdQuery(999);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.Null(result);
        mockCategoryService.Verify(s => s.GetCategoryByIdAsync(999), Times.Once);
    }
}
