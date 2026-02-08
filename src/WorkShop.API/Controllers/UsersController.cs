using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Commands.Users;
using WorkShop.Application.Queries.Users;

namespace WorkShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseModel>>> GetAllUsers()
    {
        var users = await _mediator.Send(new GetAllUsersQuery());
        return Ok(users);
    }

    [HttpPost("{id}/block")]
    public async Task<ActionResult> BlockUser(int id)
    {
        await _mediator.Send(new BlockUserCommand(id));
        return Ok(new { message = "User blocked successfully" });
    }

    [HttpPost("{id}/unblock")]
    public async Task<ActionResult> UnblockUser(int id)
    {
        await _mediator.Send(new UnblockUserCommand(id));
        return Ok(new { message = "User unblocked successfully" });
    }
}
