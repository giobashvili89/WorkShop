using Microsoft.AspNetCore.Mvc;
using MediatR;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Commands.Auth;

namespace WorkShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseModel>> Register([FromBody] RegisterRequestModel registerDto)
    {
        var result = await _mediator.Send(new RegisterCommand(registerDto));
        if (result == null)
            return BadRequest("User already exists");
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseModel>> Login([FromBody] LoginRequestModel loginDto)
    {
        var result = await _mediator.Send(new LoginCommand(loginDto));
        if (result == null)
            return Unauthorized("Invalid username or password");
        return Ok(result);
    }
}
