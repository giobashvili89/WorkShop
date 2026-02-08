using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponseModel>> GetAllCategoriesAsync();
    Task<CategoryResponseModel?> GetCategoryByIdAsync(int id);
    Task<CategoryResponseModel> CreateCategoryAsync(CategoryRequestModel category);
    Task<CategoryResponseModel?> UpdateCategoryAsync(int id, CategoryRequestModel category);
    Task<bool> DeleteCategoryAsync(int id);
}
