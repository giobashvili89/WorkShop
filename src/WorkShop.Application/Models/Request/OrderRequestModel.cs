namespace WorkShop.Application.Models.Request;

public class OrderRequestModel
{
    public List<OrderItemRequestModel> Items { get; set; } = new();
}

public class OrderItemRequestModel
{
    public int BookId { get; set; }
    public int Quantity { get; set; }
}
