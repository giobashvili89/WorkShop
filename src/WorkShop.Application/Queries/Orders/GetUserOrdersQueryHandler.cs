using MediatR;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Orders;

public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, IEnumerable<OrderResponseModel>>
{
    private readonly IOrderService _orderService;

    public GetUserOrdersQueryHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IEnumerable<OrderResponseModel>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _orderService.GetUserOrdersAsync(request.UserId);
    }
}
