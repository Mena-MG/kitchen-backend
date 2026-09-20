using System.Security.Claims;
using KitchenApi.DTOs.Orders;
using KitchenApi.DTOs.Products;
using KitchenApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Place a new order or submit quotation
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var order = await _orderService.CreateOrderAsync(dto, userId);

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    /// <summary>
    /// Get single order details by Order ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<OrderResponseDto>> GetOrder(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isOwner = User.IsInRole("Owner");

        var order = await _orderService.GetOrderByIdAsync(id, userId, isOwner);
        if (order == null)
        {
            return NotFound(new { message = $"Order '{id}' was not found." });
        }
        return Ok(order);
    }

    /// <summary>
    /// Get order history for the logged-in customer
    /// </summary>
    [HttpGet("my-orders")]
    [Authorize]
    public async Task<ActionResult<List<OrderResponseDto>>> GetMyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var orders = await _orderService.GetUserOrdersAsync(userId);
        return Ok(orders);
    }

    /// <summary>
    /// Get all orders with status filter (Owner only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<PagedResult<OrderResponseDto>>> GetAllOrders(
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var orders = await _orderService.GetAllOrdersAsync(status, pageNumber, pageSize);
        return Ok(orders);
    }

    /// <summary>
    /// Update order status (Owner only)
    /// </summary>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<OrderResponseDto>> UpdateStatus(string id, [FromBody] UpdateOrderStatusDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _orderService.UpdateOrderStatusAsync(id, dto.Status);
        if (updated == null)
        {
            return NotFound(new { message = $"Order '{id}' was not found." });
        }
        return Ok(updated);
    }
}
