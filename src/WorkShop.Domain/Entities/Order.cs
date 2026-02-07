namespace WorkShop.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled
    public string TrackingStatus { get; set; } = "Order Placed"; // Order Placed, Processing, In Warehouse, On The Way, Out for Delivery, Delivered
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedDate { get; set; }
    public bool EmailSent { get; set; } = false;
    
    // Delivery information
    public string PhoneNumber { get; set; } = string.Empty;
    public string AlternativePhoneNumber { get; set; } = string.Empty;
    public string HomeAddress { get; set; } = string.Empty;
    
    // Navigation property
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
