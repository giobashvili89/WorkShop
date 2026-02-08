using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using WorkShop.Application.Models.Request;
using Xunit;

namespace WorkShop.API.Tests;

public class ErrorHandlingIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ErrorHandlingIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetBook_WhenBookDoesNotExist_Returns404WithErrorMessage()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/books/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("not found", content.ToLower());
        Assert.Contains("statusCode", content);
        Assert.Contains("error", content);
    }

    [Fact]
    public async Task Login_WithInvalidUsername_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginDto = new LoginRequestModel
        {
            Username = "nonexistentuser",
            Password = "somepassword"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("not found", content.ToLower());
    }

    [Fact]
    public async Task Register_WithExistingUser_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // First registration
        var registerDto = new RegisterRequestModel
        {
            Username = $"testuser_{Guid.NewGuid()}",
            Email = $"test_{Guid.NewGuid()}@example.com",
            Password = "Password123!"
        };
        
        await client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Act - Try to register with same username
        var response = await client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("already exists", content.ToLower());
    }
}
