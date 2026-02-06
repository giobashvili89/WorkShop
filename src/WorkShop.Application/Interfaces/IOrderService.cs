using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponseModel?> CreateOrderAsync(int userId, OrderRequestModel orderDto);
    Task<OrderResponseModel?> GetOrderByIdAsync(int orderId);
    Task<IEnumerable<OrderResponseModel>> GetUserOrdersAsync(int userId);
    Task<IEnumerable<OrderResponseModel>> GetAllOrdersAsync();
    Task<bool> CancelOrderAsync(int orderId, int userId);
}
