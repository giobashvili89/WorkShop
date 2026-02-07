using WorkShop.Domain.Entities;

namespace WorkShop.Application.Interfaces;

public interface IEmailService
{
    Task SendOrderConfirmationEmailAsync(Order order, User user);
    Task SendOrderCancellationEmailAsync(Order order, User user);
}
