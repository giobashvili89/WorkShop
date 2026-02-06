using System.Collections.Concurrent;
using WorkShop.Application.Models;
using WorkShop.Application.Interfaces;
using WorkShop.Domain.Entities;

namespace WorkShop.Infrastructure.Services;

public class ProductService : IProductService
{
    // In-memory storage for simplicity - using ConcurrentBag for thread safety
    private static readonly ConcurrentBag<Product> _products = new(new[]
    {
        new Product { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 999.99m },
        new Product { Id = 2, Name = "Mouse", Description = "Wireless mouse", Price = 29.99m },
        new Product { Id = 3, Name = "Keyboard", Description = "Mechanical keyboard", Price = 79.99m }
    });
    private static int _nextId = 4;

    public Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = _products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price
        });
        return Task.FromResult(products);
    }

    public Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
            return Task.FromResult<ProductDto?>(null);

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price
        };
        return Task.FromResult<ProductDto?>(productDto);
    }

    public Task<ProductDto> CreateProductAsync(ProductDto productDto)
    {
        var product = new Product
        {
            Id = Interlocked.Increment(ref _nextId) - 1,
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price
        };
        _products.Add(product);

        productDto.Id = product.Id;
        return Task.FromResult(productDto);
    }

    public Task<ProductDto?> UpdateProductAsync(int id, ProductDto productDto)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
            return Task.FromResult<ProductDto?>(null);

        product.Name = productDto.Name;
        product.Description = productDto.Description;
        product.Price = productDto.Price;

        productDto.Id = id;
        return Task.FromResult<ProductDto?>(productDto);
    }

    public Task<bool> DeleteProductAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
            return Task.FromResult(false);

        // ConcurrentBag doesn't support Remove, so we'll use TryTake pattern
        var updatedProducts = _products.Where(p => p.Id != id).ToList();
        while (_products.TryTake(out _)) { }
        foreach (var p in updatedProducts)
        {
            _products.Add(p);
        }
        return Task.FromResult(true);
    }
}
