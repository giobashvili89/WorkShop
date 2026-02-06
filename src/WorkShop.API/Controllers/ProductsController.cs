using Microsoft.AspNetCore.Mvc;
using WorkShop.Application.Models;
using WorkShop.Application.Interfaces;

namespace WorkShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductDto productDto)
    {
        var createdProduct = await _productService.CreateProductAsync(productDto);
        return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] ProductDto productDto)
    {
        var updatedProduct = await _productService.UpdateProductAsync(id, productDto);
        if (updatedProduct == null)
            return NotFound();
        return Ok(updatedProduct);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
