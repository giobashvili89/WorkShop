using FluentValidation.TestHelper;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Validators;

namespace WorkShop.Application.Tests;

public class BookRequestModelValidatorTests
{
    private readonly BookRequestModelValidator _validator;

    public BookRequestModelValidatorTests()
    {
        _validator = new BookRequestModelValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var model = new BookRequestModel { Title = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Title_Exceeds_MaxLength()
    {
        var model = new BookRequestModel { Title = new string('A', 201) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title cannot exceed 200 characters.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Title_Is_Valid()
    {
        var model = new BookRequestModel { Title = "Valid Title" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Author_Is_Empty()
    {
        var model = new BookRequestModel { Author = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Author)
            .WithErrorMessage("Author is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Author_Exceeds_MaxLength()
    {
        var model = new BookRequestModel { Author = new string('A', 101) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Author)
            .WithErrorMessage("Author cannot exceed 100 characters.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Author_Is_Valid()
    {
        var model = new BookRequestModel { Author = "Valid Author" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Author);
    }

    [Fact]
    public void Should_Have_Error_When_CategoryId_Is_Zero()
    {
        var model = new BookRequestModel { CategoryId = 0 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId)
            .WithErrorMessage("CategoryId must be greater than 0.");
    }

    [Fact]
    public void Should_Have_Error_When_CategoryId_Is_Negative()
    {
        var model = new BookRequestModel { CategoryId = -1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId)
            .WithErrorMessage("CategoryId must be greater than 0.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_CategoryId_Is_Valid()
    {
        var model = new BookRequestModel { CategoryId = 1 };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_MaxLength()
    {
        var model = new BookRequestModel { Description = new string('A', 1001) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description cannot exceed 1000 characters.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Valid()
    {
        var model = new BookRequestModel { Description = "Valid description" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Null()
    {
        var model = new BookRequestModel { Description = null };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Have_Error_When_ISBN_Is_Empty()
    {
        var model = new BookRequestModel { ISBN = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ISBN)
            .WithErrorMessage("ISBN is required.");
    }

    [Fact]
    public void Should_Have_Error_When_ISBN_Exceeds_MaxLength()
    {
        var model = new BookRequestModel { ISBN = new string('1', 21) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ISBN)
            .WithErrorMessage("ISBN cannot exceed 20 characters.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_ISBN_Is_Valid()
    {
        var model = new BookRequestModel { ISBN = "978-3-16-148410-0" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.ISBN);
    }

    [Fact]
    public void Should_Have_Error_When_Price_Is_Zero()
    {
        var model = new BookRequestModel { Price = 0 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage("Price must be greater than 0.");
    }

    [Fact]
    public void Should_Have_Error_When_Price_Is_Negative()
    {
        var model = new BookRequestModel { Price = -10.50m };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage("Price must be greater than 0.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Price_Is_Positive()
    {
        var model = new BookRequestModel { Price = 19.99m };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Should_Have_Error_When_StockQuantity_Is_Negative()
    {
        var model = new BookRequestModel { StockQuantity = -1 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.StockQuantity)
            .WithErrorMessage("Stock quantity cannot be negative.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_StockQuantity_Is_Zero()
    {
        var model = new BookRequestModel { StockQuantity = 0 };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.StockQuantity);
    }

    [Fact]
    public void Should_Not_Have_Error_When_StockQuantity_Is_Positive()
    {
        var model = new BookRequestModel { StockQuantity = 100 };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.StockQuantity);
    }

    [Fact]
    public void Should_Have_Error_When_PublishedDate_Is_Empty()
    {
        var model = new BookRequestModel { PublishedDate = default };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PublishedDate)
            .WithErrorMessage("Published date is required.");
    }

    [Fact]
    public void Should_Have_Error_When_PublishedDate_Is_In_Future()
    {
        var model = new BookRequestModel { PublishedDate = DateTime.UtcNow.AddDays(1) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PublishedDate)
            .WithErrorMessage("Published date cannot be in the future.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_PublishedDate_Is_Today()
    {
        // Use a date slightly in the past to avoid race condition with DateTime.UtcNow comparison
        var model = new BookRequestModel { PublishedDate = DateTime.UtcNow.AddSeconds(-1) };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.PublishedDate);
    }

    [Fact]
    public void Should_Not_Have_Error_When_PublishedDate_Is_In_Past()
    {
        var model = new BookRequestModel { PublishedDate = DateTime.UtcNow.AddYears(-1) };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.PublishedDate);
    }

    [Fact]
    public void Should_Pass_Validation_When_All_Fields_Are_Valid()
    {
        var model = new BookRequestModel
        {
            Title = "The Great Gatsby",
            Author = "F. Scott Fitzgerald",
            CategoryId = 1,
            Description = "A novel set in the Jazz Age that explores themes of wealth, love, and the American Dream.",
            ISBN = "978-0-7432-7356-5",
            Price = 15.99m,
            StockQuantity = 50,
            PublishedDate = new DateTime(1925, 4, 10)
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
