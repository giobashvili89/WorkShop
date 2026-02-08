using MediatR;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Orders;

public record GetOrderByIdQuery(int Id) : IRequest<OrderResponseModel?>;
