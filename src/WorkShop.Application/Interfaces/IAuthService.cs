using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseModel?> RegisterAsync(RegisterRequestModel registerDto);
    Task<AuthResponseModel?> LoginAsync(LoginRequestModel loginDto);
}
