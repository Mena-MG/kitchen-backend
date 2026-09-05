using System.ComponentModel.DataAnnotations;

namespace KitchenApi.DTOs.Promos;

public class ValidatePromoDto
{
    [Required]
    public string Code { get; set; } = string.Empty;

    [Range(0, 10000000)]
    public decimal Subtotal { get; set; }
}

public class PromoValidationResultDto
{
    public bool IsValid { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int DiscountPercent { get; set; }
    public decimal FixedDiscount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NewTotal { get; set; }
    public string Label { get; set; } = string.Empty;
    public string LabelAr { get; set; } = string.Empty;
}

public class CreatePromoDto
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    public int DiscountPercent { get; set; } = 0;

    public decimal FixedDiscount { get; set; } = 0;

    public string Label { get; set; } = string.Empty;

    public string LabelAr { get; set; } = string.Empty;

    public decimal MinOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}
