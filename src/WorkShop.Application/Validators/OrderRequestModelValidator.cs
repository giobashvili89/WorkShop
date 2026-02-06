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
    }
}
