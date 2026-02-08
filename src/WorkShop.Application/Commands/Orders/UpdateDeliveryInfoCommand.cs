using MediatR;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Commands.Orders;

public record UpdateDeliveryInfoCommand(int OrderId, UpdateDeliveryInfoRequestModel DeliveryInfo) : IRequest<OrderResponseModel?>;
