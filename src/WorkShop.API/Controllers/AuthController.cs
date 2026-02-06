using Microsoft.AspNetCore.Mvc;
using WorkShop.Application.Models;
using WorkShop.Application.Interfaces;

namespace WorkShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseModel>> Register([FromBody] RegisterRequestModel registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);
        if (result == null)
            return BadRequest("User already exists");
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseModel>> Login([FromBody] LoginRequestModel loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        if (result == null)
            return Unauthorized("Invalid username or password");
        return Ok(result);
    }
}
