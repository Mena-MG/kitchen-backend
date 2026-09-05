using KitchenApi.DTOs.Products;
using KitchenApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// List products with search, category filtering, and sorting
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<ProductResponseDto>>> GetProducts([FromQuery] ProductQueryParameters query)
    {
        var result = await _productService.GetProductsAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Get single product by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductResponseDto>> GetProduct(string id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound(new { message = $"Product with ID '{id}' was not found." });
        }
        return Ok(product);
    }

    /// <summary>
    /// Create new product (Owner only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<ProductResponseDto>> CreateProduct([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update product (Owner only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<ProductResponseDto>> UpdateProduct(string id, [FromBody] UpdateProductDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _productService.UpdateProductAsync(id, dto);
        if (updated == null)
        {
            return NotFound(new { message = $"Product with ID '{id}' was not found." });
        }
        return Ok(updated);
    }

    /// <summary>
    /// Delete product (Owner only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        var deleted = await _productService.DeleteProductAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = $"Product with ID '{id}' was not found." });
        }
        return NoContent();
    }

    /// <summary>
    /// Re-seed default catalog materials (Owner only)
    /// </summary>
    [HttpPost("seed")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> SeedProducts()
    {
        await _productService.SeedDefaultProductsAsync();
        return Ok(new { message = "Catalog seeded successfully." });
    }
}
