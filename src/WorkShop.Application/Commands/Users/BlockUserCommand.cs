using MediatR;

namespace WorkShop.Application.Commands.Users;

public record BlockUserCommand(int UserId) : IRequest;
