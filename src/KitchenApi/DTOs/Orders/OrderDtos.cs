using System.ComponentModel.DataAnnotations;

namespace KitchenApi.DTOs.Orders;

public class CreateOrderItemDto
{
    [Required]
    public string ProductId { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string ProductNameAr { get; set; } = string.Empty;

    [Range(0.01, 1000000)]
    public decimal UnitPrice { get; set; }

    [Range(0.01, 10000)]
    public double Quantity { get; set; }

    public string UnitType { get; set; } = string.Empty;

    public string CustomNote { get; set; } = string.Empty;
}

public class CreateOrderDto
{
    [Required]
    [MaxLength(150)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string CustomerPhone { get; set; } = string.Empty;

    public string DeliveryMethod { get; set; } = "delivery"; // delivery, pickup

    public string ShippingAddress { get; set; } = string.Empty;

    public string ShippingCity { get; set; } = string.Empty;

    public string PickupLocationId { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = "instapay"; // instapay, cod, pickup

    public string TransactionRef { get; set; } = string.Empty;

    public string InstapayReceiptBase64 { get; set; } = string.Empty;

    public string AppliedPromoCode { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "Order must contain at least one item.")]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class OrderResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string DeliveryMethod { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
    public string PickupLocationId { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionRef { get; set; } = string.Empty;
    public string InstapayReceiptBase64 { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public string AppliedPromoCode { get; set; } = string.Empty;
    public decimal DeliveryFee { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}

public class OrderItemResponseDto
{
    public int Id { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductNameAr { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public double Quantity { get; set; }
    public string UnitType { get; set; } = string.Empty;
    public string CustomNote { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}

public class UpdateOrderStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty; // Pending, Confirmed, InProduction, Delivered, Cancelled
}
