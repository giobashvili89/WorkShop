using FluentValidation;
using WorkShop.Application.Models.Request;
using WorkShop.Domain.Enums;

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

        // Enum validation is now handled by the type system
        // Status and TrackingStatus are already validated by being nullable enums
    }
}
