using System.ComponentModel.DataAnnotations;

namespace KitchenApi.Models;

public class Review
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ProductId { get; set; } = string.Empty;

    public Product? Product { get; set; }

    public string? UserId { get; set; }

    public AppUser? User { get; set; }

    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    [MaxLength(1000)]
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
