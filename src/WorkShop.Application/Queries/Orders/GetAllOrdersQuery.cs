using MediatR;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Orders;

public record GetAllOrdersQuery(
    string? Status = null,
    string? TrackingStatus = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string? CustomerSearch = null,
    int? OrderId = null,
    decimal? MinAmount = null,
    decimal? MaxAmount = null
) : IRequest<IEnumerable<OrderResponseModel>>;
