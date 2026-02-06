using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Interfaces;

namespace WorkShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseModel>> CreateOrder([FromBody] OrderRequestModel orderDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        var order = await _orderService.CreateOrderAsync(userId, orderDto);
        
        if (order == null)
            return BadRequest("Unable to create order. Please check stock availability.");
        
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseModel>> GetOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        
        // Users can only see their own orders unless they're admin
        if (userRole != "Admin" && userIdClaim != null && order.UserId != int.Parse(userIdClaim.Value))
            return Forbid();

        return Ok(order);
    }

    [HttpGet("my-orders")]
    public async Task<ActionResult<IEnumerable<OrderResponseModel>>> GetMyOrders()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        var orders = await _orderService.GetUserOrdersAsync(userId);
        return Ok(orders);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<OrderResponseModel>>> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> CancelOrder(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        var result = await _orderService.CancelOrderAsync(id, userId);
        
        if (!result)
            return NotFound();
        
        return NoContent();
    }
}
