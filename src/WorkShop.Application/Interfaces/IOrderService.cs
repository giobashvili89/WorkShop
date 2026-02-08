using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponseModel?> CreateOrderAsync(int userId, OrderRequestModel orderDto);
    Task<OrderResponseModel?> GetOrderByIdAsync(int orderId);
    Task<IEnumerable<OrderResponseModel>> GetUserOrdersAsync(int userId);
    Task<IEnumerable<OrderResponseModel>> GetAllOrdersAsync(string? status = null, string? trackingStatus = null, 
        DateTime? startDate = null, DateTime? endDate = null, string? customerSearch = null, 
        int? orderId = null, decimal? minAmount = null, decimal? maxAmount = null);
    Task<bool> CancelOrderAsync(int orderId, int userId);
    Task<OrderResponseModel?> UpdateDeliveryInfoAsync(int orderId, UpdateDeliveryInfoRequestModel model);
}
