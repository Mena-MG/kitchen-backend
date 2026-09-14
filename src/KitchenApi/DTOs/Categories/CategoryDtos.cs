using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace KitchenApi.DTOs.Categories;

public class CreateCategoryDto
{
    public string? Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string NameAr { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public IFormFile? IconFile { get; set; }

    public string Slug { get; set; } = string.Empty;

    public int DisplayOrder { get; set; } = 0;
}

public class CategoryResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public int ProductsCount { get; set; }
}
