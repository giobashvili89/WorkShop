using FluentValidation;
using WorkShop.Application.Models.Request;

namespace WorkShop.Application.Validators;

public class OrderRequestModelValidator : AbstractValidator<OrderRequestModel>
{
    public OrderRequestModelValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must contain at least one item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.BookId)
                .GreaterThan(0).WithMessage("Book ID must be greater than 0.");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        });

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[0-9\s\-()]{7,20}$").WithMessage("Phone number must be a valid format (7-20 characters, can include +, spaces, -, and parentheses).");

        RuleFor(x => x.AlternativePhoneNumber)
            .NotEmpty().WithMessage("Alternative phone number is required.")
            .Matches(@"^\+?[0-9\s\-()]{7,20}$").WithMessage("Alternative phone number must be a valid format (7-20 characters, can include +, spaces, -, and parentheses).");

        RuleFor(x => x.HomeAddress)
            .NotEmpty().WithMessage("Home address is required.")
            .MinimumLength(10).WithMessage("Home address must be at least 10 characters.")
            .MaximumLength(500).WithMessage("Home address must not exceed 500 characters.");
    }
}
