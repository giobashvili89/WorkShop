using WorkShop.Domain.Enums;

namespace WorkShop.Application.Models.Request;

public class UpdateDeliveryInfoRequestModel
{
    public TrackingStatus? TrackingStatus { get; set; }
    public OrderStatus? Status { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AlternativePhoneNumber { get; set; }
    public string? HomeAddress { get; set; }
    public bool SendEmail { get; set; } = false;
}
