using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitchenApi.Models;

public class PromoCode
{
    [Key]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    public int DiscountPercent { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal FixedDiscount { get; set; } = 0;

    [MaxLength(200)]
    public string Label { get; set; } = string.Empty;

    [MaxLength(200)]
    public string LabelAr { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal MinOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
