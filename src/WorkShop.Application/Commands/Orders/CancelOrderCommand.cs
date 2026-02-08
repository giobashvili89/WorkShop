using MediatR;

namespace WorkShop.Application.Commands.Orders;

public record CancelOrderCommand(int OrderId, int UserId) : IRequest<bool>;
