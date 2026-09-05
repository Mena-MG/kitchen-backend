using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitchenApi.Models;

public class Product
{
    [Key]
    public string Id { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string NameAr { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OriginalPrice { get; set; }

    [MaxLength(50)]
    public string UnitType { get; set; } = "per_sqm"; // per_sqm, per_linear_meter, per_piece, per_set

    [MaxLength(50)]
    public string SurfaceType { get; set; } = "none"; // countertop, cabinet, backsplash, accessory, none

    [MaxLength(100)]
    public string? CategoryId { get; set; }

    public Category? Category { get; set; }

    [MaxLength(150)]
    public string CategoryName { get; set; } = string.Empty;

    [MaxLength(150)]
    public string CategoryNameAr { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Material { get; set; } = string.Empty;

    [MaxLength(150)]
    public string MaterialAr { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Finish { get; set; } = string.Empty;

    [MaxLength(150)]
    public string FinishAr { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Color { get; set; } = string.Empty;

    [MaxLength(150)]
    public string ColorAr { get; set; } = string.Empty;

    [MaxLength(20)]
    public string ColorHex { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Thickness { get; set; } = string.Empty;

    [MaxLength(100)]
    public string OriginCountry { get; set; } = string.Empty;

    [MaxLength(100)]
    public string OriginCountryAr { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public string VideoUrl { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Badge { get; set; } = string.Empty;

    [MaxLength(100)]
    public string BadgeAr { get; set; } = string.Empty;

    public bool IsFeatured { get; set; } = false;

    public bool IsBestSeller { get; set; } = false;

    public bool InStock { get; set; } = true;

    public double Rating { get; set; } = 5.0;

    public int ReviewsCount { get; set; } = 1;

    public string ShortDescription { get; set; } = string.Empty;

    public string ShortDescriptionAr { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string DescriptionAr { get; set; } = string.Empty;

    public string FeaturesJson { get; set; } = "[]";

    public string FeaturesArJson { get; set; } = "[]";

    public string GalleryJson { get; set; } = "[]";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public List<Review> Reviews { get; set; } = new();
}
