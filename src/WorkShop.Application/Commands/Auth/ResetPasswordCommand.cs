using MediatR;
using WorkShop.Application.Models.Request;

namespace WorkShop.Application.Commands.Auth;

public record ResetPasswordCommand(ResetPasswordRequestModel Request) : IRequest;
