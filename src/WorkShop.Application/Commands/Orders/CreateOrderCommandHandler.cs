using MediatR;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Commands.Orders;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponseModel?>
{
    private readonly IOrderService _orderService;

    public CreateOrderCommandHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<OrderResponseModel?> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        return await _orderService.CreateOrderAsync(request.UserId, request.Order);
    }
}
