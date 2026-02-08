using MediatR;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Orders;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderResponseModel?>
{
    private readonly IOrderService _orderService;

    public GetOrderByIdQueryHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<OrderResponseModel?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        return await _orderService.GetOrderByIdAsync(request.Id);
    }
}
