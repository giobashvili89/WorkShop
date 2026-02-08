using Moq;
using Xunit;
using WorkShop.Application.Interfaces;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Application.Commands.Auth;
using WorkShop.Domain.Enums;

namespace WorkShop.Application.Tests.Handlers;

public class AuthCommandHandlersTests
{
    [Fact]
    public async Task RegisterCommandHandler_RegistersUser_WhenUserDoesNotExist()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        var registerRequest = new RegisterRequestModel
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        };
        var expectedResponse = new AuthResponseModel
        {
            Token = "test-token",
            Username = registerRequest.Username,
            Role = UserRole.Customer,
            UserId = 1
        };
        mockAuthService.Setup(s => s.RegisterAsync(registerRequest)).ReturnsAsync(expectedResponse);
        
        var handler = new RegisterCommandHandler(mockAuthService.Object);
        var command = new RegisterCommand(registerRequest);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-token", result.Token);
        Assert.Equal("testuser", result.Username);
        mockAuthService.Verify(s => s.RegisterAsync(registerRequest), Times.Once);
    }

    [Fact]
    public async Task RegisterCommandHandler_ReturnsNull_WhenUserAlreadyExists()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        var registerRequest = new RegisterRequestModel
        {
            Username = "existinguser",
            Email = "existing@example.com",
            Password = "Password123!"
        };
        mockAuthService.Setup(s => s.RegisterAsync(registerRequest)).ReturnsAsync((AuthResponseModel?)null);
        
        var handler = new RegisterCommandHandler(mockAuthService.Object);
        var command = new RegisterCommand(registerRequest);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.Null(result);
        mockAuthService.Verify(s => s.RegisterAsync(registerRequest), Times.Once);
    }

    [Fact]
    public async Task LoginCommandHandler_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        var loginRequest = new LoginRequestModel
        {
            Username = "testuser",
            Password = "Password123!"
        };
        var expectedResponse = new AuthResponseModel
        {
            Token = "test-token",
            Username = loginRequest.Username,
            Role = UserRole.Customer,
            UserId = 1
        };
        mockAuthService.Setup(s => s.LoginAsync(loginRequest)).ReturnsAsync(expectedResponse);
        
        var handler = new LoginCommandHandler(mockAuthService.Object);
        var command = new LoginCommand(loginRequest);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-token", result.Token);
        Assert.Equal("testuser", result.Username);
        mockAuthService.Verify(s => s.LoginAsync(loginRequest), Times.Once);
    }

    [Fact]
    public async Task LoginCommandHandler_ReturnsNull_WhenCredentialsAreInvalid()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        var loginRequest = new LoginRequestModel
        {
            Username = "testuser",
            Password = "WrongPassword"
        };
        mockAuthService.Setup(s => s.LoginAsync(loginRequest)).ReturnsAsync((AuthResponseModel?)null);
        
        var handler = new LoginCommandHandler(mockAuthService.Object);
        var command = new LoginCommand(loginRequest);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.Null(result);
        mockAuthService.Verify(s => s.LoginAsync(loginRequest), Times.Once);
    }

    [Fact]
    public async Task ForgotPasswordCommandHandler_ReturnsResetToken_WhenEmailExists()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        var forgotPasswordRequest = new ForgotPasswordRequestModel
        {
            Email = "test@example.com"
        };
        var expectedToken = "reset-token-12345";
        mockAuthService.Setup(s => s.ForgotPasswordAsync(forgotPasswordRequest)).ReturnsAsync(expectedToken);
        
        var handler = new ForgotPasswordCommandHandler(mockAuthService.Object);
        var command = new ForgotPasswordCommand(forgotPasswordRequest);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedToken, result);
        mockAuthService.Verify(s => s.ForgotPasswordAsync(forgotPasswordRequest), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordCommandHandler_ResetsPassword_WhenTokenIsValid()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        var resetPasswordRequest = new ResetPasswordRequestModel
        {
            Token = "valid-token",
            NewPassword = "NewPassword123!"
        };
        mockAuthService.Setup(s => s.ResetPasswordAsync(resetPasswordRequest)).Returns(Task.CompletedTask);
        
        var handler = new ResetPasswordCommandHandler(mockAuthService.Object);
        var command = new ResetPasswordCommand(resetPasswordRequest);
        
        // Act
        await handler.Handle(command, CancellationToken.None);
        
        // Assert
        mockAuthService.Verify(s => s.ResetPasswordAsync(resetPasswordRequest), Times.Once);
    }
}
