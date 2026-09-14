using KitchenApi.Data;
using KitchenApi.DTOs.Categories;
using KitchenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace KitchenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public CategoriesController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
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
    [Consumes("multipart/form-data")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<CategoryResponseDto>> CreateCategory([FromForm] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            if (dto.IconFile != null)
            {
                dto.Icon = await SaveUploadedFileAsync(dto.IconFile, "images");
            }
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

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
    [Consumes("multipart/form-data")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<CategoryResponseDto>> UpdateCategory(string id, [FromForm] CreateCategoryDto dto)
    {
        try
        {
            if (dto.IconFile != null)
            {
                dto.Icon = await SaveUploadedFileAsync(dto.IconFile, "images");
            }
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

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

    private async Task<string> SaveUploadedFileAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
        {
            return string.Empty;
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg", ".gif" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(ext))
        {
            throw new InvalidOperationException($"Unsupported icon format. Allowed extensions: {string.Join(", ", allowedExtensions)}");
        }

        if (file.Length > 15 * 1024 * 1024)
        {
            throw new InvalidOperationException("Icon file exceeds the 15MB size limit.");
        }

        var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", folderName);
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"icon_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid().ToString("N")[..8]}{ext}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var request = HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        return $"{baseUrl}/uploads/{folderName}/{uniqueFileName}";
    }
}
