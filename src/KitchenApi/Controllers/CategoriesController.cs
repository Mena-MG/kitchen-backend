using KitchenApi.Data;
using KitchenApi.DTOs.Categories;
using KitchenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KitchenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// List all categories with product counts
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<CategoryResponseDto>>> GetCategories()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                NameAr = c.NameAr,
                Icon = c.Icon,
                Slug = c.Slug,
                DisplayOrder = c.DisplayOrder,
                ProductsCount = c.Products.Count
            })
            .ToListAsync();

        return Ok(categories);
    }

    /// <summary>
    /// Create new category (Owner only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<CategoryResponseDto>> CreateCategory([FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var id = !string.IsNullOrWhiteSpace(dto.Id)
            ? dto.Id
            : $"cat-{dto.Slug?.ToLower().Replace(" ", "-") ?? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()}";

        var category = new Category
        {
            Id = id,
            Name = dto.Name,
            NameAr = dto.NameAr,
            Icon = dto.Icon,
            Slug = string.IsNullOrWhiteSpace(dto.Slug) ? dto.Name.ToLower().Replace(" ", "-") : dto.Slug,
            DisplayOrder = dto.DisplayOrder
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            NameAr = category.NameAr,
            Icon = category.Icon,
            Slug = category.Slug,
            DisplayOrder = category.DisplayOrder,
            ProductsCount = 0
        });
    }

    /// <summary>
    /// Update category (Owner only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<CategoryResponseDto>> UpdateCategory(string id, [FromBody] CreateCategoryDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound(new { message = $"Category '{id}' not found." });

        category.Name = dto.Name;
        category.NameAr = dto.NameAr;
        category.Icon = dto.Icon;
        category.Slug = dto.Slug;
        category.DisplayOrder = dto.DisplayOrder;

        await _context.SaveChangesAsync();

        return Ok(new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            NameAr = category.NameAr,
            Icon = category.Icon,
            Slug = category.Slug,
            DisplayOrder = category.DisplayOrder,
            ProductsCount = await _context.Products.CountAsync(p => p.CategoryId == id)
        });
    }

    /// <summary>
    /// Delete category (Owner only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound(new { message = $"Category '{id}' not found." });

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
