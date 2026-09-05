using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KitchenApi.Models;

public class Order
{
    [Key]
    public string Id { get; set; } = string.Empty; // e.g. AUR-KITCHEN-58291

    public string? UserId { get; set; }

    public AppUser? User { get; set; }

    [Required]
    [MaxLength(150)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string CustomerPhone { get; set; } = string.Empty;

    [MaxLength(50)]
    public string DeliveryMethod { get; set; } = "delivery"; // delivery, pickup

    [MaxLength(300)]
    public string ShippingAddress { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ShippingCity { get; set; } = string.Empty;

    [MaxLength(100)]
    public string PickupLocationId { get; set; } = string.Empty;

    [MaxLength(50)]
    public string PaymentMethod { get; set; } = "instapay"; // instapay, cod, pickup

    [MaxLength(100)]
    public string TransactionRef { get; set; } = string.Empty;

    public string InstapayReceiptBase64 { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    [MaxLength(50)]
    public string AppliedPromoCode { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal DeliveryFee { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, InProduction, Delivered, Cancelled

    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string OrderId { get; set; } = string.Empty;

    public Order? Order { get; set; }

    [Required]
    public string ProductId { get; set; } = string.Empty;

    public Product? Product { get; set; }

    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string ProductNameAr { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    public double Quantity { get; set; }

    [MaxLength(50)]
    public string UnitType { get; set; } = string.Empty;

    [MaxLength(250)]
    public string CustomNote { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }
}
