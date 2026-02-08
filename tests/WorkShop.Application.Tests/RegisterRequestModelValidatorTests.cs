using FluentValidation.TestHelper;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Validators;

namespace WorkShop.Application.Tests;

public class RegisterRequestModelValidatorTests
{
    private readonly RegisterRequestModelValidator _validator;

    public RegisterRequestModelValidatorTests()
    {
        _validator = new RegisterRequestModelValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Username_Is_Empty()
    {
        var model = new RegisterRequestModel { Username = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Username_Is_Null()
    {
        var model = new RegisterRequestModel { Username = null };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Username_Is_Too_Short()
    {
        var model = new RegisterRequestModel { Username = "ab" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username must be at least 3 characters long.");
    }

    [Fact]
    public void Should_Have_Error_When_Username_Exceeds_MaxLength()
    {
        var model = new RegisterRequestModel { Username = new string('A', 51) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username cannot exceed 50 characters.");
    }

    [Fact]
    public void Should_Have_Error_When_Username_Contains_Invalid_Characters()
    {
        var model = new RegisterRequestModel { Username = "user@name" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username can only contain letters, numbers, and underscores.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Username_Is_Valid()
    {
        var model = new RegisterRequestModel { Username = "valid_user123" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Username_Contains_Underscores()
    {
        var model = new RegisterRequestModel { Username = "user_name_123" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var model = new RegisterRequestModel { Email = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Null()
    {
        var model = new RegisterRequestModel { Email = null };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var model = new RegisterRequestModel { Email = "invalidemail" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("A valid email address is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Exceeds_MaxLength()
    {
        // Create email with more than 100 characters
        var model = new RegisterRequestModel 
        { 
            Email = new string('a', 92) + "@test.com", // 92 + 9 = 101 characters
            Username = "validuser123",
            Password = "validpass123"
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email cannot exceed 100 characters.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Email_Is_Valid()
    {
        var model = new RegisterRequestModel { Email = "user@example.com" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var model = new RegisterRequestModel { Password = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Null()
    {
        var model = new RegisterRequestModel { Password = null };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        var model = new RegisterRequestModel { Password = "12345" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 6 characters long.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Exceeds_MaxLength()
    {
        var model = new RegisterRequestModel { Password = new string('A', 101) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password cannot exceed 100 characters.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Password_Is_Valid()
    {
        var model = new RegisterRequestModel { Password = "securepassword123" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Password_Is_Minimum_Length()
    {
        var model = new RegisterRequestModel { Password = "123456" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Pass_Validation_When_All_Fields_Are_Valid()
    {
        var model = new RegisterRequestModel
        {
            Username = "newuser123",
            Email = "newuser@example.com",
            Password = "securepass123"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
