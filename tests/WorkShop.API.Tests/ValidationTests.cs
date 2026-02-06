using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using WorkShop.Application.DTOs;

namespace WorkShop.API.Tests;

public class ValidationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ValidationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_WithEmptyUsername_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registerDto = new RegisterDto
        {
            Username = "",
            Email = "test@example.com",
            Password = "password123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Username is required", content);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registerDto = new RegisterDto
        {
            Username = "testuser",
            Email = "invalid-email",
            Password = "password123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("valid email address", content);
    }

    [Fact]
    public async Task Register_WithShortPassword_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registerDto = new RegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "12345"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("at least 6 characters", content);
    }

    [Fact]
    public async Task Login_WithEmptyUsername_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginDto = new LoginDto
        {
            Username = "",
            Password = "password123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Username is required", content);
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginDto = new LoginDto
        {
            Username = "testuser",
            Password = ""
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Password is required", content);
    }
}
