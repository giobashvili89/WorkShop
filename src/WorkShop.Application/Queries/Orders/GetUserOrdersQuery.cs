using MediatR;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Orders;

public record GetUserOrdersQuery(int UserId) : IRequest<IEnumerable<OrderResponseModel>>;
