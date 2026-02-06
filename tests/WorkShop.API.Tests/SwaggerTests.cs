using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace WorkShop.API.Tests;

public class SwaggerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SwaggerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Swagger_Endpoint_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/index.html");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Swagger_Json_Endpoint_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Swagger_Json_ContainsApiTitle()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("WorkShop.API", content);
    }

    [Fact]
    public async Task Swagger_Json_ContainsProductsEndpoints()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("/api/Products", content);
        Assert.Contains("\"get\"", content);
        Assert.Contains("\"post\"", content);
        Assert.Contains("\"put\"", content);
        Assert.Contains("\"delete\"", content);
    }

    [Fact]
    public async Task Swagger_Json_ContainsProductDto_Schema()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("ProductDto", content);
        Assert.Contains("\"schemas\"", content);
    }

    [Fact]
    public async Task SwaggerUI_Resources_AreAccessible()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var cssResponse = await client.GetAsync("/swagger/swagger-ui.css");
        var jsResponse = await client.GetAsync("/swagger/swagger-ui-bundle.js");

        // Assert
        Assert.Equal(HttpStatusCode.OK, cssResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, jsResponse.StatusCode);
    }
}
