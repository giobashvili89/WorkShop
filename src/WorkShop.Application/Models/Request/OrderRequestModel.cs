namespace WorkShop.Application.Models.Request;

public class OrderRequestModel
{
    public List<OrderItemRequestModel> Items { get; set; } = new();
    public string PhoneNumber { get; set; } = string.Empty;
    public string AlternativePhoneNumber { get; set; } = string.Empty;
    public string HomeAddress { get; set; } = string.Empty;
}

public class OrderItemRequestModel
{
    public int BookId { get; set; }
    public int Quantity { get; set; }
}
