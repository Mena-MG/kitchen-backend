using KitchenApi.DTOs.Orders;
using KitchenApi.DTOs.Products;

namespace KitchenApi.Services;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto, string? userId = null);
    Task<OrderResponseDto?> GetOrderByIdAsync(string id, string? userId = null, bool isOwner = false);
    Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId);
    Task<PagedResult<OrderResponseDto>> GetAllOrdersAsync(string? status, int pageNumber = 1, int pageSize = 20);
    Task<OrderResponseDto?> UpdateOrderStatusAsync(string id, string status);
}
