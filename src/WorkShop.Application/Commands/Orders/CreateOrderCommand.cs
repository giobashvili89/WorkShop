using MediatR;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Commands.Orders;

public record CreateOrderCommand(int UserId, OrderRequestModel Order) : IRequest<OrderResponseModel?>;
