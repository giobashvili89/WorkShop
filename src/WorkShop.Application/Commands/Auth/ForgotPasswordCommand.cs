using MediatR;
using WorkShop.Application.Models.Request;

namespace WorkShop.Application.Commands.Auth;

public record ForgotPasswordCommand(ForgotPasswordRequestModel Request) : IRequest<string>;
