using System.ComponentModel.DataAnnotations;

namespace KitchenApi.Models;

public class Category
{
    [Key]
    public string Id { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string NameAr { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Icon { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Slug { get; set; } = string.Empty;

    public int DisplayOrder { get; set; } = 0;

    public List<Product> Products { get; set; } = new();
}
