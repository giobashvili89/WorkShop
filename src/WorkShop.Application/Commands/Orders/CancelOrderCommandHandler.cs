using MediatR;
using WorkShop.Application.Interfaces;

namespace WorkShop.Application.Commands.Orders;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IOrderService _orderService;

    public CancelOrderCommandHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        return await _orderService.CancelOrderAsync(request.OrderId, request.UserId);
    }
}
