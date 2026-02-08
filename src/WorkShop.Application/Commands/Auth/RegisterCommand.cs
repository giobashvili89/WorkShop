using MediatR;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Commands.Auth;

public record RegisterCommand(RegisterRequestModel RegisterInfo) : IRequest<AuthResponseModel?>;
