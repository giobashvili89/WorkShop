using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WorkShop.Application.Models.Request;
using WorkShop.Application.Models.Response;
using WorkShop.Domain.Entities;
using WorkShop.Domain.Exceptions;
using WorkShop.Infrastructure.Data;
using WorkShop.Infrastructure.Services;

namespace WorkShop.Infrastructure.Tests;

public class AuthServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private IConfiguration GetConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"Jwt:Secret", "YourSuperSecretKeyForJWTTokenGeneration12345"},
            {"Jwt:Issuer", "WorkShopAPI"},
            {"Jwt:Audience", "WorkShopClient"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
    }

    [Fact]
    public async Task RegisterAsync_CreatesNewUser_ReturnsAuthResponse()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var configuration = GetConfiguration();
        var service = new AuthService(context, configuration);
        var registerDto = new RegisterRequestModel
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };

        // Act
        var result = await service.RegisterAsync(registerDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
        Assert.NotEmpty(result.Token);
        var usersInDb = await context.Users.ToListAsync();
        Assert.Single(usersInDb);
    }

    [Fact]
    public async Task RegisterAsync_ThrowsBadRequestException_WhenUsernameExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var configuration = GetConfiguration();
        var existingUser = new User
        {
            Username = "existinguser",
            Email = "existing@example.com",
            PasswordHash = "hash"
        };
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        var service = new AuthService(context, configuration);
        var registerDto = new RegisterRequestModel
        {
            Username = "existinguser",
            Email = "newemail@example.com",
            Password = "password123"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(
            async () => await service.RegisterAsync(registerDto));
        Assert.Equal("User with this username or email already exists.", exception.Message);
    }

    [Fact]
    public async Task RegisterAsync_ThrowsBadRequestException_WhenEmailExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var configuration = GetConfiguration();
        var existingUser = new User
        {
            Username = "existinguser",
            Email = "existing@example.com",
            PasswordHash = "hash"
        };
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        var service = new AuthService(context, configuration);
        var registerDto = new RegisterRequestModel
        {
            Username = "newuser",
            Email = "existing@example.com",
            Password = "password123"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(
            async () => await service.RegisterAsync(registerDto));
        Assert.Equal("User with this username or email already exists.", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_ReturnsAuthResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var configuration = GetConfiguration();
        var service = new AuthService(context, configuration);

        // First register a user
        var registerDto = new RegisterRequestModel
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };
        await service.RegisterAsync(registerDto);

        var loginDto = new LoginRequestModel
        {
            Username = "testuser",
            Password = "password123"
        };

        // Act
        var result = await service.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task LoginAsync_ThrowsUserNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var configuration = GetConfiguration();
        var service = new AuthService(context, configuration);

        var loginDto = new LoginRequestModel
        {
            Username = "nonexistentuser",
            Password = "password123"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UserNotFoundException>(
            async () => await service.LoginAsync(loginDto));
        Assert.Equal("User with username 'nonexistentuser' was not found.", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_ThrowsUnauthorizedException_WhenPasswordIsIncorrect()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var configuration = GetConfiguration();
        var service = new AuthService(context, configuration);

        // First register a user
        var registerDto = new RegisterRequestModel
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "correctpassword"
        };
        await service.RegisterAsync(registerDto);

        var loginDto = new LoginRequestModel
        {
            Username = "testuser",
            Password = "wrongpassword"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(
            async () => await service.LoginAsync(loginDto));
        Assert.Equal("Invalid username or password.", exception.Message);
    }
}
