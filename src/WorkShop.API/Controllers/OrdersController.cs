using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediatR;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Commands.Orders;
using WorkShop.Application.Queries.Orders;

namespace WorkShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseModel>> CreateOrder([FromBody] OrderRequestModel orderDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        var order = await _mediator.Send(new CreateOrderCommand(userId, orderDto));
        
        if (order == null)
            return BadRequest("Unable to create order. Please check stock availability.");
        
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseModel>> GetOrder(int id)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id));
        if (order == null)
            return NotFound();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        
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
        var orders = await _mediator.Send(new GetUserOrdersQuery(userId));
        return Ok(orders);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<OrderResponseModel>>> GetAllOrders(
        [FromQuery] string? status = null,
        [FromQuery] string? trackingStatus = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? customerSearch = null,
        [FromQuery] int? orderId = null,
        [FromQuery] decimal? minAmount = null,
        [FromQuery] decimal? maxAmount = null)
    {
        var orders = await _mediator.Send(new GetAllOrdersQuery(status, trackingStatus, startDate, endDate, 
            customerSearch, orderId, minAmount, maxAmount));
        return Ok(orders);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> CancelOrder(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        
        try
        {
            var result = await _mediator.Send(new CancelOrderCommand(id, userId));
            
            if (!result)
                return NotFound();
            
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/delivery")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OrderResponseModel>> UpdateDeliveryInfo(int id, [FromBody] UpdateDeliveryInfoRequestModel model)
    {
        var order = await _mediator.Send(new UpdateDeliveryInfoCommand(id, model));
        
        if (order == null)
            return NotFound();
        
        return Ok(order);
    }
}
