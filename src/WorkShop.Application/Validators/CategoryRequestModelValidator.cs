using FluentValidation;
using WorkShop.Application.Models.Request;

namespace WorkShop.Application.Validators;

public class CategoryRequestModelValidator : AbstractValidator<CategoryRequestModel>
{
    public CategoryRequestModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(50).WithMessage("Category name cannot exceed 50 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}
