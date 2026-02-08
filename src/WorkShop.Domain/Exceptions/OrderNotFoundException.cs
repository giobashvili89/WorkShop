namespace WorkShop.Domain.Exceptions;

/// <summary>
/// Exception thrown when an order is not found
/// </summary>
public class OrderNotFoundException : NotFoundException
{
    public OrderNotFoundException(int orderId) 
        : base($"Order with ID {orderId} was not found.")
    {
    }
}
