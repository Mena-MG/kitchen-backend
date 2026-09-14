using KitchenApi.DTOs.Products;
using KitchenApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace KitchenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IWebHostEnvironment _env;

    public ProductsController(IProductService productService, IWebHostEnvironment env)
    {
        _productService = productService;
        _env = env;
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
    [Consumes("multipart/form-data")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<ProductResponseDto>> CreateProduct([FromForm] CreateProductDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await ProcessUploadedMediaAsync(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        var created = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update product (Owner only)
    /// </summary>
    [HttpPut("{id}")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<ProductResponseDto>> UpdateProduct(string id, [FromForm] UpdateProductDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await ProcessUploadedMediaAsync(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

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

    private async Task ProcessUploadedMediaAsync(CreateProductDto dto)
    {
        var galleryUrls = dto.Gallery ?? new List<string>();

        if (dto.ImageFile != null)
        {
            dto.ImageUrl = await SaveUploadedFileAsync(dto.ImageFile, "images");
        }

        if (dto.VideoFile != null)
        {
            dto.VideoUrl = await SaveUploadedFileAsync(dto.VideoFile, "videos");
        }

        if (dto.GalleryFiles != null)
        {
            foreach (var file in dto.GalleryFiles)
            {
                if (file == null || file.Length == 0) continue;

                galleryUrls.Add(await SaveUploadedFileAsync(file, "images"));
            }
        }

        dto.Gallery = galleryUrls;
    }

    private async Task<string> SaveUploadedFileAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
        {
            return string.Empty;
        }

        var allowedExtensions = folderName == "videos"
            ? new[] { ".mp4", ".webm", ".mov", ".mkv" }
            : new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg", ".gif" };

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
        {
            throw new InvalidOperationException($"Unsupported {folderName.TrimEnd('s')} format. Allowed extensions: {string.Join(", ", allowedExtensions)}");
        }

        if (file.Length > (folderName == "videos" ? 100 * 1024 * 1024 : 15 * 1024 * 1024))
        {
            throw new InvalidOperationException($"{char.ToUpper(folderName[0])}{folderName.Substring(1)} file exceeds the size limit.");
        }

        var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", folderName);
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{(folderName == "videos" ? "vid" : "img")}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid().ToString("N")[..8]}{ext}";
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
