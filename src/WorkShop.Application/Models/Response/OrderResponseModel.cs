namespace WorkShop.Application.Models.Response;

public class OrderResponseModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string TrackingStatus { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public bool CanCancel { get; set; } // Based on 1-hour rule
    public List<OrderItemResponseModel> Items { get; set; } = new();
}

public class OrderItemResponseModel
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
