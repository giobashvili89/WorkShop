namespace WorkShop.Application.Models.Request;

public class UpdateDeliveryInfoRequestModel
{
    public string? TrackingStatus { get; set; }
    public string? Status { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AlternativePhoneNumber { get; set; }
    public string? HomeAddress { get; set; }
    public bool SendEmail { get; set; } = false;
}
