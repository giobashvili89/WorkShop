using WorkShop.Domain.Enums;

namespace WorkShop.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public TrackingStatus TrackingStatus { get; set; } = TrackingStatus.OrderPlaced;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedDate { get; set; }
    public bool EmailSent { get; set; } = false;
    
    public string PhoneNumber { get; set; } = string.Empty;
    public string AlternativePhoneNumber { get; set; } = string.Empty;
    public string HomeAddress { get; set; } = string.Empty;
    
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
