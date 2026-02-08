using FluentValidation;
using WorkShop.Application.Models.Request;

namespace WorkShop.Application.Validators;

public class UpdateDeliveryInfoRequestModelValidator : AbstractValidator<UpdateDeliveryInfoRequestModel>
{
    public UpdateDeliveryInfoRequestModelValidator()
    {
        When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[0-9\s\-()]{7,20}$")
                .WithMessage("Phone number must be between 7 and 20 characters and can contain digits, spaces, hyphens, parentheses, and optional leading plus sign.");
        });

        When(x => !string.IsNullOrEmpty(x.AlternativePhoneNumber), () =>
        {
            RuleFor(x => x.AlternativePhoneNumber)
                .Matches(@"^\+?[0-9\s\-()]{7,20}$")
                .WithMessage("Alternative phone number must be between 7 and 20 characters and can contain digits, spaces, hyphens, parentheses, and optional leading plus sign.");
        });

        When(x => !string.IsNullOrEmpty(x.HomeAddress), () =>
        {
            RuleFor(x => x.HomeAddress)
                .MinimumLength(10)
                .WithMessage("Address must be at least 10 characters long.");
        });

        When(x => !string.IsNullOrEmpty(x.Status), () =>
        {
            RuleFor(x => x.Status)
                .Must(s => s == "Pending" || s == "Completed" || s == "Cancelled")
                .WithMessage("Status must be one of: Pending, Completed, Cancelled");
        });

        When(x => !string.IsNullOrEmpty(x.TrackingStatus), () =>
        {
            RuleFor(x => x.TrackingStatus)
                .Must(ts => ts == "Order Placed" || ts == "Processing" || ts == "In Warehouse" || 
                           ts == "On The Way" || ts == "Out for Delivery" || ts == "Delivered")
                .WithMessage("Tracking status must be one of: Order Placed, Processing, In Warehouse, On The Way, Out for Delivery, Delivered");
        });
    }
}
