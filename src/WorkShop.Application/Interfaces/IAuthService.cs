using WorkShop.Application.Models;

namespace WorkShop.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseModel?> RegisterAsync(RegisterRequestModel registerDto);
    Task<AuthResponseModel?> LoginAsync(LoginRequestModel loginDto);
}
