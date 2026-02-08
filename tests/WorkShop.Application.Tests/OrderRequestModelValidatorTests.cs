using FluentValidation.TestHelper;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Validators;

namespace WorkShop.Application.Tests;

public class OrderRequestModelValidatorTests
{
    private readonly OrderRequestModelValidator _validator;

    public OrderRequestModelValidatorTests()
    {
        _validator = new OrderRequestModelValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Items_Is_Empty()
    {
        var model = new OrderRequestModel { Items = new List<OrderItemRequestModel>() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("Order must contain at least one item.");
    }

    [Fact]
    public void Should_Have_Error_When_Items_Is_Null()
    {
        var model = new OrderRequestModel { Items = null };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("Order must contain at least one item.");
    }

    [Fact]
    public void Should_Have_Error_When_Item_BookId_Is_Zero()
    {
        var model = new OrderRequestModel
        {
            Items = new List<OrderItemRequestModel>
            {
                new OrderItemRequestModel { BookId = 0, Quantity = 1 }
            }
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor("Items[0].BookId")
            .WithErrorMessage("Book ID must be greater than 0.");
    }

    [Fact]
    public void Should_Have_Error_When_Item_BookId_Is_Negative()
    {
        var model = new OrderRequestModel
        {
            Items = new List<OrderItemRequestModel>
            {
                new OrderItemRequestModel { BookId = -1, Quantity = 1 }
            }
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor("Items[0].BookId")
            .WithErrorMessage("Book ID must be greater than 0.");
    }

    [Fact]
    public void Should_Have_Error_When_Item_Quantity_Is_Zero()
    {
        var model = new OrderRequestModel
        {
            Items = new List<OrderItemRequestModel>
            {
                new OrderItemRequestModel { BookId = 1, Quantity = 0 }
            }
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor("Items[0].Quantity")
            .WithErrorMessage("Quantity must be greater than 0.");
    }

    [Fact]
    public void Should_Have_Error_When_Item_Quantity_Is_Negative()
    {
        var model = new OrderRequestModel
        {
            Items = new List<OrderItemRequestModel>
            {
                new OrderItemRequestModel { BookId = 1, Quantity = -5 }
            }
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor("Items[0].Quantity")
            .WithErrorMessage("Quantity must be greater than 0.");
    }

    [Fact]
    public void Should_Have_Error_When_PhoneNumber_Is_Empty()
    {
        var model = new OrderRequestModel { PhoneNumber = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number is required.");
    }

    [Fact]
    public void Should_Have_Error_When_PhoneNumber_Is_Invalid_Format()
    {
        var model = new OrderRequestModel { PhoneNumber = "abc123" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number must be a valid format (7-20 characters, can include +, spaces, -, and parentheses).");
    }

    [Fact]
    public void Should_Have_Error_When_PhoneNumber_Is_Too_Short()
    {
        var model = new OrderRequestModel { PhoneNumber = "12345" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number must be a valid format (7-20 characters, can include +, spaces, -, and parentheses).");
    }

    [Fact]
    public void Should_Have_Error_When_PhoneNumber_Is_Too_Long()
    {
        var model = new OrderRequestModel { PhoneNumber = "123456789012345678901" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number must be a valid format (7-20 characters, can include +, spaces, -, and parentheses).");
    }

    [Fact]
    public void Should_Not_Have_Error_When_PhoneNumber_Is_Valid()
    {
        var model = new OrderRequestModel { PhoneNumber = "+1 (555) 123-4567" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Should_Not_Have_Error_When_PhoneNumber_Is_Valid_Simple()
    {
        var model = new OrderRequestModel { PhoneNumber = "1234567890" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Should_Have_Error_When_AlternativePhoneNumber_Is_Empty()
    {
        var model = new OrderRequestModel { AlternativePhoneNumber = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AlternativePhoneNumber)
            .WithErrorMessage("Alternative phone number is required.");
    }

    [Fact]
    public void Should_Have_Error_When_AlternativePhoneNumber_Is_Invalid_Format()
    {
        var model = new OrderRequestModel { AlternativePhoneNumber = "invalid-phone" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AlternativePhoneNumber)
            .WithErrorMessage("Alternative phone number must be a valid format (7-20 characters, can include +, spaces, -, and parentheses).");
    }

    [Fact]
    public void Should_Not_Have_Error_When_AlternativePhoneNumber_Is_Valid()
    {
        var model = new OrderRequestModel { AlternativePhoneNumber = "+995 555 123456" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.AlternativePhoneNumber);
    }

    [Fact]
    public void Should_Have_Error_When_HomeAddress_Is_Empty()
    {
        var model = new OrderRequestModel { HomeAddress = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.HomeAddress)
            .WithErrorMessage("Home address is required.");
    }

    [Fact]
    public void Should_Have_Error_When_HomeAddress_Is_Too_Short()
    {
        var model = new OrderRequestModel { HomeAddress = "Short" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.HomeAddress)
            .WithErrorMessage("Home address must be at least 10 characters.");
    }

    [Fact]
    public void Should_Have_Error_When_HomeAddress_Is_Too_Long()
    {
        var model = new OrderRequestModel { HomeAddress = new string('A', 501) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.HomeAddress)
            .WithErrorMessage("Home address must not exceed 500 characters.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_HomeAddress_Is_Valid()
    {
        var model = new OrderRequestModel { HomeAddress = "123 Main Street, Apt 4B, New York, NY 10001" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.HomeAddress);
    }

    [Fact]
    public void Should_Pass_Validation_When_All_Fields_Are_Valid()
    {
        var model = new OrderRequestModel
        {
            Items = new List<OrderItemRequestModel>
            {
                new OrderItemRequestModel { BookId = 1, Quantity = 2 },
                new OrderItemRequestModel { BookId = 2, Quantity = 1 }
            },
            PhoneNumber = "+1 (555) 123-4567",
            AlternativePhoneNumber = "+1 (555) 987-6543",
            HomeAddress = "123 Main Street, Apt 4B, New York, NY 10001"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Validate_Multiple_Items_Correctly()
    {
        var model = new OrderRequestModel
        {
            Items = new List<OrderItemRequestModel>
            {
                new OrderItemRequestModel { BookId = 1, Quantity = 2 },
                new OrderItemRequestModel { BookId = 0, Quantity = 1 }, // Invalid BookId
                new OrderItemRequestModel { BookId = 3, Quantity = -1 }  // Invalid Quantity
            }
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor("Items[1].BookId");
        result.ShouldHaveValidationErrorFor("Items[2].Quantity");
    }
}
