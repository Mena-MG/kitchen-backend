using System.ComponentModel.DataAnnotations;

namespace KitchenApi.DTOs.Reviews;

public class CreateReviewDto
{
    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    [MaxLength(1000)]
    public string Comment { get; set; } = string.Empty;
}

public class ReviewResponseDto
{
    public int Id { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
