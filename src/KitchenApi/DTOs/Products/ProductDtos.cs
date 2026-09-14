using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace KitchenApi.DTOs.Products;

public class CreateProductDto
{
    public string? Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string NameAr { get; set; } = string.Empty;

    [Range(0, 1000000)]
    public decimal Price { get; set; }

    [Range(0, 1000000)]
    public decimal OriginalPrice { get; set; }

    public string UnitType { get; set; } = "per_sqm";

    public string SurfaceType { get; set; } = "none"; // countertop, cabinet, backsplash, accessory, none

    public string? CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string CategoryNameAr { get; set; } = string.Empty;

    public string Material { get; set; } = string.Empty;

    public string MaterialAr { get; set; } = string.Empty;

    public string Finish { get; set; } = string.Empty;

    public string FinishAr { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string ColorAr { get; set; } = string.Empty;

    public string ColorHex { get; set; } = string.Empty;

    public string Thickness { get; set; } = string.Empty;

    public string OriginCountry { get; set; } = string.Empty;

    public string OriginCountryAr { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public string VideoUrl { get; set; } = string.Empty;

    public IFormFile? ImageFile { get; set; }

    public IFormFile? VideoFile { get; set; }

    public List<IFormFile>? GalleryFiles { get; set; }

    public string Badge { get; set; } = string.Empty;

    public string BadgeAr { get; set; } = string.Empty;

    public bool IsFeatured { get; set; } = false;

    public bool IsBestSeller { get; set; } = false;

    public bool InStock { get; set; } = true;

    public string ShortDescription { get; set; } = string.Empty;

    public string ShortDescriptionAr { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string DescriptionAr { get; set; } = string.Empty;

    public List<string>? Features { get; set; }

    public List<string>? FeaturesAr { get; set; }

    public List<string>? Gallery { get; set; }
}

public class UpdateProductDto : CreateProductDto
{
}

public class ProductResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal OriginalPrice { get; set; }
    public string UnitType { get; set; } = string.Empty;
    public string SurfaceType { get; set; } = "none";
    public string? CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryNameAr { get; set; } = string.Empty;
    public string Material { get; set; } = string.Empty;
    public string MaterialAr { get; set; } = string.Empty;
    public string Finish { get; set; } = string.Empty;
    public string FinishAr { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string ColorAr { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public string Thickness { get; set; } = string.Empty;
    public string OriginCountry { get; set; } = string.Empty;
    public string OriginCountryAr { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
    public string BadgeAr { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool IsBestSeller { get; set; }
    public bool InStock { get; set; }
    public double Rating { get; set; }
    public int ReviewsCount { get; set; }
    public string ShortDescription { get; set; } = string.Empty;
    public string ShortDescriptionAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
    public List<string> FeaturesAr { get; set; } = new();
    public List<string> Gallery { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class ProductQueryParameters
{
    public string? Search { get; set; }
    public string? Category { get; set; }
    public string? SurfaceType { get; set; }
    public string? Material { get; set; }
    public bool? Featured { get; set; }
    public bool? BestSeller { get; set; }
    public bool? InStock { get; set; }
    public string? SortBy { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
