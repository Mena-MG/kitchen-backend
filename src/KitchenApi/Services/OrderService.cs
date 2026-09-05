using KitchenApi.Data;
using KitchenApi.DTOs.Orders;
using KitchenApi.DTOs.Products;
using KitchenApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KitchenApi.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrderService> _logger;

    public const decimal FreeDeliveryThreshold = 5000;
    public const decimal DefaultDeliveryFee = 100;

    public OrderService(AppDbContext context, ILogger<OrderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto, string? userId = null)
    {
        var orderId = $"AUR-KITCHEN-{new Random().Next(10000, 99999)}";

        // Calculate Subtotal and build order items
        decimal subtotal = 0;
        var orderItems = new List<OrderItem>();

        foreach (var itemDto in dto.Items)
        {
            var product = await _context.Products.FindAsync(itemDto.ProductId);
            var unitPrice = product != null ? product.Price : itemDto.UnitPrice;
            var productName = product != null ? product.Name : itemDto.ProductName;
            var productNameAr = product != null ? product.NameAr : itemDto.ProductNameAr;
            var unitType = product != null ? product.UnitType : itemDto.UnitType;

            var totalPrice = unitPrice * (decimal)itemDto.Quantity;
            subtotal += totalPrice;

            orderItems.Add(new OrderItem
            {
                OrderId = orderId,
                ProductId = itemDto.ProductId,
                ProductName = productName,
                ProductNameAr = productNameAr,
                UnitPrice = unitPrice,
                Quantity = itemDto.Quantity,
                UnitType = unitType,
                CustomNote = itemDto.CustomNote,
                TotalPrice = totalPrice
            });
        }

        // Promo Discount
        decimal discountAmount = 0;
        if (!string.IsNullOrWhiteSpace(dto.AppliedPromoCode))
        {
            var promo = await _context.PromoCodes
                .FirstOrDefaultAsync(p => p.Code.ToUpper() == dto.AppliedPromoCode.Trim().ToUpper() && p.IsActive);

            if (promo != null && subtotal >= promo.MinOrder)
            {
                if (promo.DiscountPercent > 0)
                {
                    discountAmount = Math.Round(subtotal * promo.DiscountPercent / 100m, 2);
                }
                else if (promo.FixedDiscount > 0)
                {
                    discountAmount = Math.Min(promo.FixedDiscount, subtotal);
                }
            }
        }

        // Delivery Fee
        decimal deliveryFee = 0;
        if (dto.DeliveryMethod?.ToLower() == "delivery")
        {
            var discountedSubtotal = subtotal - discountAmount;
            deliveryFee = discountedSubtotal >= FreeDeliveryThreshold ? 0 : DefaultDeliveryFee;
        }

        var total = Math.Max(0, subtotal - discountAmount + deliveryFee);

        var order = new Order
        {
            Id = orderId,
            UserId = userId,
            CustomerName = dto.CustomerName,
            CustomerPhone = dto.CustomerPhone,
            DeliveryMethod = dto.DeliveryMethod ?? "delivery",
            ShippingAddress = dto.ShippingAddress ?? string.Empty,
            ShippingCity = dto.ShippingCity ?? string.Empty,
            PickupLocationId = dto.PickupLocationId ?? string.Empty,
            PaymentMethod = dto.PaymentMethod ?? "instapay",
            TransactionRef = dto.TransactionRef ?? string.Empty,
            InstapayReceiptBase64 = dto.InstapayReceiptBase64 ?? string.Empty,
            Subtotal = subtotal,
            DiscountAmount = discountAmount,
            AppliedPromoCode = dto.AppliedPromoCode ?? string.Empty,
            DeliveryFee = deliveryFee,
            Total = total,
            Status = "Pending",
            Notes = dto.Notes ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            Items = orderItems
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        _logger.LogInformation("New Order Created: {OrderId}, Total: {Total} EGP", order.Id, order.Total);

        return MapToResponseDto(order);
    }

    public async Task<OrderResponseDto?> GetOrderByIdAsync(string id, string? userId = null, bool isOwner = false)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .AsNoTracking();

        var order = await query.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return null;

        // Security check: if not owner and order has a userId, only the owner or that user can view it
        if (!isOwner && !string.IsNullOrEmpty(order.UserId) && order.UserId != userId)
        {
            return null;
        }

        return MapToResponseDto(order);
    }

    public async Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId)
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

        return orders.Select(MapToResponseDto).ToList();
    }

    public async Task<PagedResult<OrderResponseDto>> GetAllOrdersAsync(string? status, int pageNumber = 1, int pageSize = 20)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(o => o.Status.ToLower() == status.Trim().ToLower());
        }

        var totalCount = await query.CountAsync();
        var pNum = Math.Max(1, pageNumber);
        var pSize = Math.Clamp(pageSize, 1, 100);

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pNum - 1) * pSize)
            .Take(pSize)
            .ToListAsync();

        return new PagedResult<OrderResponseDto>
        {
            Items = orders.Select(MapToResponseDto).ToList(),
            TotalCount = totalCount,
            PageNumber = pNum,
            PageSize = pSize
        };
    }

    public async Task<OrderResponseDto?> UpdateOrderStatusAsync(string id, string status)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return null;

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Order {Id} status updated to {Status}", id, status);

        return MapToResponseDto(order);
    }

    private static OrderResponseDto MapToResponseDto(Order o) => new()
    {
        Id = o.Id,
        UserId = o.UserId,
        CustomerName = o.CustomerName,
        CustomerPhone = o.CustomerPhone,
        DeliveryMethod = o.DeliveryMethod,
        ShippingAddress = o.ShippingAddress,
        ShippingCity = o.ShippingCity,
        PickupLocationId = o.PickupLocationId,
        PaymentMethod = o.PaymentMethod,
        TransactionRef = o.TransactionRef,
        InstapayReceiptBase64 = o.InstapayReceiptBase64,
        Subtotal = o.Subtotal,
        DiscountAmount = o.DiscountAmount,
        AppliedPromoCode = o.AppliedPromoCode,
        DeliveryFee = o.DeliveryFee,
        Total = o.Total,
        Status = o.Status,
        Notes = o.Notes,
        CreatedAt = o.CreatedAt,
        Items = o.Items.Select(i => new OrderItemResponseDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            ProductNameAr = i.ProductNameAr,
            UnitPrice = i.UnitPrice,
            Quantity = i.Quantity,
            UnitType = i.UnitType,
            CustomNote = i.CustomNote,
            TotalPrice = i.TotalPrice
        }).ToList()
    };
}
