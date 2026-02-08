using MediatR;
using WorkShop.Application.Interfaces;

namespace WorkShop.Application.Commands.Auth;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, string>
{
    private readonly IAuthService _authService;

    public ForgotPasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<string> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        return await _authService.ForgotPasswordAsync(request.Request);
    }
}
