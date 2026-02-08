using MediatR;

namespace WorkShop.Application.Commands.Users;

public record UnblockUserCommand(int UserId) : IRequest;
