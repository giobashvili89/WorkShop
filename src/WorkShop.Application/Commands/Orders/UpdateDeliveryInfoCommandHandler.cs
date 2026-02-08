using MediatR;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Commands.Orders;

public class UpdateDeliveryInfoCommandHandler : IRequestHandler<UpdateDeliveryInfoCommand, OrderResponseModel?>
{
    private readonly IOrderService _orderService;

    public UpdateDeliveryInfoCommandHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<OrderResponseModel?> Handle(UpdateDeliveryInfoCommand request, CancellationToken cancellationToken)
    {
        return await _orderService.UpdateDeliveryInfoAsync(request.OrderId, request.DeliveryInfo);
    }
}
