using WorkShop.Application.DTOs;

namespace WorkShop.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(ProductDto product);
    Task<ProductDto?> UpdateProductAsync(int id, ProductDto product);
    Task<bool> DeleteProductAsync(int id);
}
