using System.Security.Claims;
using KitchenApi.Data;
using KitchenApi.DTOs.Reviews;
using KitchenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KitchenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReviewsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all reviews for a specific product
    /// </summary>
    [HttpGet("product/{productId}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ReviewResponseDto>>> GetProductReviews(string productId)
    {
        var reviews = await _context.Reviews
            .AsNoTracking()
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewResponseDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                UserId = r.UserId,
                UserName = r.UserName,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return Ok(reviews);
    }

    /// <summary>
    /// Submit a review on a product
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ReviewResponseDto>> CreateReview([FromBody] CreateReviewDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null)
        {
            return NotFound(new { message = $"Product '{dto.ProductId}' not found." });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var review = new Review
        {
            ProductId = dto.ProductId,
            UserId = userId,
            UserName = dto.UserName,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);

        // Update product overall rating and reviewsCount
        product.ReviewsCount += 1;
        var existingRatings = await _context.Reviews
            .Where(r => r.ProductId == dto.ProductId)
            .Select(r => r.Rating)
            .ToListAsync();

        existingRatings.Add(dto.Rating);
        product.Rating = Math.Round(existingRatings.Average(), 1);

        await _context.SaveChangesAsync();

        return Ok(new ReviewResponseDto
        {
            Id = review.Id,
            ProductId = review.ProductId,
            UserId = review.UserId,
            UserName = review.UserName,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        });
    }
}
