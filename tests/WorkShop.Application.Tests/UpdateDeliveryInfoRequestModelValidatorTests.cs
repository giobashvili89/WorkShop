using FluentValidation.TestHelper;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Validators;
using WorkShop.Domain.Enums;

namespace WorkShop.Application.Tests;

public class UpdateDeliveryInfoRequestModelValidatorTests
{
    private readonly UpdateDeliveryInfoRequestModelValidator _validator;

    public UpdateDeliveryInfoRequestModelValidatorTests()
    {
        _validator = new UpdateDeliveryInfoRequestModelValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_PhoneNumber_Is_Null()
    {
        var model = new UpdateDeliveryInfoRequestModel { PhoneNumber = null };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Should_Not_Have_Error_When_PhoneNumber_Is_Empty()
    {
        var model = new UpdateDeliveryInfoRequestModel { PhoneNumber = "" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Should_Have_Error_When_PhoneNumber_Is_Invalid_Format()
    {
        var model = new UpdateDeliveryInfoRequestModel { PhoneNumber = "abc123" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number must be between 7 and 20 characters and can contain digits, spaces, hyphens, parentheses, and optional leading plus sign.");
    }

    [Fact]
    public void Should_Have_Error_When_PhoneNumber_Is_Too_Short()
    {
        var model = new UpdateDeliveryInfoRequestModel { PhoneNumber = "12345" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number must be between 7 and 20 characters and can contain digits, spaces, hyphens, parentheses, and optional leading plus sign.");
    }

    [Fact]
    public void Should_Have_Error_When_PhoneNumber_Is_Too_Long()
    {
        var model = new UpdateDeliveryInfoRequestModel { PhoneNumber = "123456789012345678901" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number must be between 7 and 20 characters and can contain digits, spaces, hyphens, parentheses, and optional leading plus sign.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_PhoneNumber_Is_Valid()
    {
        var model = new UpdateDeliveryInfoRequestModel { PhoneNumber = "+1 (555) 123-4567" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Should_Not_Have_Error_When_AlternativePhoneNumber_Is_Null()
    {
        var model = new UpdateDeliveryInfoRequestModel { AlternativePhoneNumber = null };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.AlternativePhoneNumber);
    }

    [Fact]
    public void Should_Not_Have_Error_When_AlternativePhoneNumber_Is_Empty()
    {
        var model = new UpdateDeliveryInfoRequestModel { AlternativePhoneNumber = "" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.AlternativePhoneNumber);
    }

    [Fact]
    public void Should_Have_Error_When_AlternativePhoneNumber_Is_Invalid_Format()
    {
        var model = new UpdateDeliveryInfoRequestModel { AlternativePhoneNumber = "invalid" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AlternativePhoneNumber)
            .WithErrorMessage("Alternative phone number must be between 7 and 20 characters and can contain digits, spaces, hyphens, parentheses, and optional leading plus sign.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_AlternativePhoneNumber_Is_Valid()
    {
        var model = new UpdateDeliveryInfoRequestModel { AlternativePhoneNumber = "+995 555 123456" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.AlternativePhoneNumber);
    }

    [Fact]
    public void Should_Not_Have_Error_When_HomeAddress_Is_Null()
    {
        var model = new UpdateDeliveryInfoRequestModel { HomeAddress = null };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.HomeAddress);
    }

    [Fact]
    public void Should_Not_Have_Error_When_HomeAddress_Is_Empty()
    {
        var model = new UpdateDeliveryInfoRequestModel { HomeAddress = "" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.HomeAddress);
    }

    [Fact]
    public void Should_Have_Error_When_HomeAddress_Is_Too_Short()
    {
        var model = new UpdateDeliveryInfoRequestModel { HomeAddress = "Short" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.HomeAddress)
            .WithErrorMessage("Address must be at least 10 characters long.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_HomeAddress_Is_Valid()
    {
        var model = new UpdateDeliveryInfoRequestModel { HomeAddress = "123 Main Street, Apt 4B" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.HomeAddress);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Status_Is_Null()
    {
        var model = new UpdateDeliveryInfoRequestModel { Status = null };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Status_Is_Pending()
    {
        var model = new UpdateDeliveryInfoRequestModel { Status = OrderStatus.Pending };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Status_Is_Completed()
    {
        var model = new UpdateDeliveryInfoRequestModel { Status = OrderStatus.Completed };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Status_Is_Cancelled()
    {
        var model = new UpdateDeliveryInfoRequestModel { Status = OrderStatus.Cancelled };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Should_Not_Have_Error_When_TrackingStatus_Is_Null()
    {
        var model = new UpdateDeliveryInfoRequestModel { TrackingStatus = null };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.TrackingStatus);
    }

    [Fact]
    public void Should_Not_Have_Error_When_TrackingStatus_Is_OrderPlaced()
    {
        var model = new UpdateDeliveryInfoRequestModel { TrackingStatus = TrackingStatus.OrderPlaced };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.TrackingStatus);
    }

    [Fact]
    public void Should_Not_Have_Error_When_TrackingStatus_Is_Processing()
    {
        var model = new UpdateDeliveryInfoRequestModel { TrackingStatus = TrackingStatus.Processing };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.TrackingStatus);
    }

    [Fact]
    public void Should_Not_Have_Error_When_TrackingStatus_Is_InWarehouse()
    {
        var model = new UpdateDeliveryInfoRequestModel { TrackingStatus = TrackingStatus.InWarehouse };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.TrackingStatus);
    }

    [Fact]
    public void Should_Not_Have_Error_When_TrackingStatus_Is_OnTheWay()
    {
        var model = new UpdateDeliveryInfoRequestModel { TrackingStatus = TrackingStatus.OnTheWay };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.TrackingStatus);
    }

    [Fact]
    public void Should_Not_Have_Error_When_TrackingStatus_Is_OutForDelivery()
    {
        var model = new UpdateDeliveryInfoRequestModel { TrackingStatus = TrackingStatus.OutForDelivery };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.TrackingStatus);
    }

    [Fact]
    public void Should_Not_Have_Error_When_TrackingStatus_Is_Delivered()
    {
        var model = new UpdateDeliveryInfoRequestModel { TrackingStatus = TrackingStatus.Delivered };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.TrackingStatus);
    }

    [Fact]
    public void Should_Pass_Validation_When_All_Optional_Fields_Are_Null()
    {
        var model = new UpdateDeliveryInfoRequestModel
        {
            PhoneNumber = null,
            AlternativePhoneNumber = null,
            HomeAddress = null,
            Status = null,
            TrackingStatus = null,
            SendEmail = false
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Pass_Validation_When_All_Fields_Are_Valid()
    {
        var model = new UpdateDeliveryInfoRequestModel
        {
            PhoneNumber = "+1 (555) 123-4567",
            AlternativePhoneNumber = "+1 (555) 987-6543",
            HomeAddress = "123 Main Street, Apt 4B, New York, NY 10001",
            Status = OrderStatus.Completed,
            TrackingStatus = TrackingStatus.Delivered,
            SendEmail = true
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
