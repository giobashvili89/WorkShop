using MediatR;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Commands.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseModel?>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponseModel?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _authService.LoginAsync(request.LoginInfo);
    }
}
