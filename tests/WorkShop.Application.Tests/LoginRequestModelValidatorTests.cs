using FluentValidation.TestHelper;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Validators;

namespace WorkShop.Application.Tests;

public class LoginRequestModelValidatorTests
{
    private readonly LoginRequestModelValidator _validator;

    public LoginRequestModelValidatorTests()
    {
        _validator = new LoginRequestModelValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Username_Is_Empty()
    {
        var model = new LoginRequestModel { Username = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Username_Is_Null()
    {
        var model = new LoginRequestModel { Username = null };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Username_Exceeds_MaxLength()
    {
        var model = new LoginRequestModel { Username = new string('A', 51) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username cannot exceed 50 characters.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Username_Is_Valid()
    {
        var model = new LoginRequestModel { Username = "validuser" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var model = new LoginRequestModel { Password = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Null()
    {
        var model = new LoginRequestModel { Password = null };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        var model = new LoginRequestModel { Password = "12345" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 6 characters long.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Exceeds_MaxLength()
    {
        var model = new LoginRequestModel { Password = new string('A', 101) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password cannot exceed 100 characters.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Password_Is_Valid()
    {
        var model = new LoginRequestModel { Password = "validpassword123" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Password_Is_Minimum_Length()
    {
        var model = new LoginRequestModel { Password = "123456" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Pass_Validation_When_All_Fields_Are_Valid()
    {
        var model = new LoginRequestModel
        {
            Username = "testuser",
            Password = "securepassword123"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
