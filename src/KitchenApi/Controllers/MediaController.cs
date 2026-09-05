using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<MediaController> _logger;

    public MediaController(IWebHostEnvironment env, ILogger<MediaController> logger)
    {
        _env = env;
        _logger = logger;
    }

    /// <summary>
    /// Upload product image (JPG, PNG, WebP)
    /// </summary>
    [HttpPost("upload-image")]
    [AllowAnonymous]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded." });
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg", ".gif" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(ext))
        {
            return BadRequest(new { message = $"File extension '{ext}' is not supported for images." });
        }

        if (file.Length > 15 * 1024 * 1024) // 15MB limit
        {
            return BadRequest(new { message = "Image size exceeds 15MB limit." });
        }

        var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "images");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"img_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid().ToString("N")[..8]}{ext}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var request = HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var publicUrl = $"{baseUrl}/uploads/images/{uniqueFileName}";

        _logger.LogInformation("Image uploaded successfully: {Url}", publicUrl);

        return Ok(new
        {
            url = publicUrl,
            fileName = uniqueFileName,
            size = file.Length,
            contentType = file.ContentType
        });
    }

    /// <summary>
    /// Upload craftsmanship video (MP4, WebM)
    /// </summary>
    [HttpPost("upload-video")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> UploadVideo([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded." });
        }

        var allowedExtensions = new[] { ".mp4", ".webm", ".mov", ".mkv" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(ext))
        {
            return BadRequest(new { message = $"File extension '{ext}' is not supported for videos." });
        }

        if (file.Length > 100 * 1024 * 1024) // 100MB limit
        {
            return BadRequest(new { message = "Video size exceeds 100MB limit." });
        }

        var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "videos");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"vid_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid().ToString("N")[..8]}{ext}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var request = HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var publicUrl = $"{baseUrl}/uploads/videos/{uniqueFileName}";

        _logger.LogInformation("Video uploaded successfully: {Url}", publicUrl);

        return Ok(new
        {
            url = publicUrl,
            fileName = uniqueFileName,
            size = file.Length,
            contentType = file.ContentType
        });
    }

    /// <summary>
    /// Upload Instapay payment receipt screenshot
    /// </summary>
    [HttpPost("upload-receipt")]
    [AllowAnonymous]
    public async Task<IActionResult> UploadReceipt([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded." });
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(ext))
        {
            return BadRequest(new { message = "Invalid receipt format." });
        }

        var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "receipts");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"receipt_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid().ToString("N")[..8]}{ext}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var request = HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var publicUrl = $"{baseUrl}/uploads/receipts/{uniqueFileName}";

        return Ok(new
        {
            url = publicUrl,
            fileName = uniqueFileName
        });
    }
}
