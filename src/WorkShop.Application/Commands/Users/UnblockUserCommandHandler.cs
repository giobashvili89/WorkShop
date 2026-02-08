using MediatR;
using WorkShop.Application.Interfaces;

namespace WorkShop.Application.Commands.Users;

public class UnblockUserCommandHandler : IRequestHandler<UnblockUserCommand>
{
    private readonly IUserService _userService;

    public UnblockUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task Handle(UnblockUserCommand request, CancellationToken cancellationToken)
    {
        await _userService.UnblockUserAsync(request.UserId);
    }
}
