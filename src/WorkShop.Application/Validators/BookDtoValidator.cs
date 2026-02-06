using FluentValidation;
using WorkShop.Application.DTOs;

namespace WorkShop.Application.Validators;

public class BookDtoValidator : AbstractValidator<BookDto>
{
    public BookDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required.")
            .MaximumLength(100).WithMessage("Author cannot exceed 100 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MaximumLength(50).WithMessage("Category cannot exceed 50 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.PublishedDate)
            .NotEmpty().WithMessage("Published date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Published date cannot be in the future.");
    }
}
