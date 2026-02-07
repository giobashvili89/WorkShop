using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WorkShop.Application.Interfaces;
using WorkShop.Domain.Entities;

namespace WorkShop.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendOrderConfirmationEmailAsync(Order order, User user)
    {
        try
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var fromEmail = smtpSettings["FromEmail"];
            var fromName = smtpSettings["FromName"];
            
            var subject = $"Order Confirmation - Order #{order.Id}";
            var body = $@"
                <html>
                <body>
                    <h2>Thank you for your order!</h2>
                    <p>Dear {user.Username},</p>
                    <p>Your order has been successfully placed.</p>
                    <h3>Order Details:</h3>
                    <ul>
                        <li>Order Number: #{order.Id}</li>
                        <li>Order Date: {order.OrderDate:yyyy-MM-dd HH:mm}</li>
                        <li>Total Amount: ${order.TotalAmount:F2}</li>
                        <li>Status: {order.Status}</li>
                        <li>Tracking Status: {order.TrackingStatus}</li>
                    </ul>
                    <p><strong>Note:</strong> You can cancel this order within 1 hour of placing it.</p>
                    <p>Thank you for shopping with us!</p>
                </body>
                </html>";

            await SendEmailAsync(user.Email, subject, body);
            _logger.LogInformation($"Order confirmation email sent to {user.Email} for order #{order.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send order confirmation email for order #{order.Id}");
            // Don't throw - email failure shouldn't break the order process
        }
    }

    public async Task SendOrderCancellationEmailAsync(Order order, User user)
    {
        try
        {
            var subject = $"Order Cancellation - Order #{order.Id}";
            var body = $@"
                <html>
                <body>
                    <h2>Order Cancelled</h2>
                    <p>Dear {user.Username},</p>
                    <p>Your order has been successfully cancelled.</p>
                    <h3>Cancelled Order Details:</h3>
                    <ul>
                        <li>Order Number: #{order.Id}</li>
                        <li>Order Date: {order.OrderDate:yyyy-MM-dd HH:mm}</li>
                        <li>Cancellation Date: {DateTime.UtcNow:yyyy-MM-dd HH:mm}</li>
                        <li>Total Amount: ${order.TotalAmount:F2}</li>
                    </ul>
                    <p>The amount will be refunded to your account within 3-5 business days.</p>
                    <p>If you have any questions, please contact our support team.</p>
                </body>
                </html>";

            await SendEmailAsync(user.Email, subject, body);
            _logger.LogInformation($"Order cancellation email sent to {user.Email} for order #{order.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send order cancellation email for order #{order.Id}");
            // Don't throw - email failure shouldn't break the cancellation process
        }
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        var host = smtpSettings["Host"];
        var port = int.Parse(smtpSettings["Port"] ?? "587");
        var enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true");
        var username = smtpSettings["Username"];
        var password = smtpSettings["Password"];
        var fromEmail = smtpSettings["FromEmail"];
        var fromName = smtpSettings["FromName"];

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            Credentials = new NetworkCredential(username, password)
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(toEmail);

        await client.SendMailAsync(mailMessage);
    }
}
