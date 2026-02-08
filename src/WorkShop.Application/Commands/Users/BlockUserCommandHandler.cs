using MediatR;
using WorkShop.Application.Interfaces;

namespace WorkShop.Application.Commands.Users;

public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand>
{
    private readonly IUserService _userService;

    public BlockUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        await _userService.BlockUserAsync(request.UserId);
    }
}
