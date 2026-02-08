using FluentValidation.TestHelper;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Validators;

namespace WorkShop.Application.Tests;

public class CategoryRequestModelValidatorTests
{
    private readonly CategoryRequestModelValidator _validator;

    public CategoryRequestModelValidatorTests()
    {
        _validator = new CategoryRequestModelValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var model = new CategoryRequestModel { Name = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Category name is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_MaxLength()
    {
        var model = new CategoryRequestModel { Name = new string('A', 51) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Category name cannot exceed 50 characters.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Name_Is_Valid()
    {
        var model = new CategoryRequestModel { Name = "Fiction" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_MaxLength()
    {
        var model = new CategoryRequestModel { Description = new string('A', 501) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description cannot exceed 500 characters.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Valid()
    {
        var model = new CategoryRequestModel { Description = "A valid description" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Pass_Validation_When_All_Fields_Are_Valid()
    {
        var model = new CategoryRequestModel
        {
            Name = "Science",
            Description = "Scientific books covering various scientific disciplines"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
