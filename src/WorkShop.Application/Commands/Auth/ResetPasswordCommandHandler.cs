using MediatR;
using WorkShop.Application.Interfaces;

namespace WorkShop.Application.Commands.Auth;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IAuthService _authService;

    public ResetPasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        await _authService.ResetPasswordAsync(request.Request);
    }
}
