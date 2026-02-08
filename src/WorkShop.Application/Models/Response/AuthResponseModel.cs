using WorkShop.Domain.Enums;

namespace WorkShop.Application.Models.Response;

public class AuthResponseModel
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public int UserId { get; set; }
}
