using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Text.Json;

namespace WorkShop.API.Tests;

/// <summary>
/// Tests to ensure OpenAPI/Swagger is properly configured and working
/// </summary>
public class OpenApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OpenApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(Environments.Development);
        });
    }

    [Fact]
    public async Task OpenApi_Endpoint_ShouldBeAccessible_InDevelopmentEnvironment()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/openapi/v1.json");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task OpenApi_Document_ShouldContainValidStructure()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/openapi/v1.json");
        var content = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(content);

        // Assert
        document.RootElement.TryGetProperty("openapi", out var openApiVersion).Should().BeTrue();
        openApiVersion.GetString().Should().NotBeNullOrEmpty();
        openApiVersion.GetString().Should().Match("3.*", "Should be OpenAPI version 3.x");

        document.RootElement.TryGetProperty("info", out var info).Should().BeTrue();
        document.RootElement.TryGetProperty("paths", out var paths).Should().BeTrue();
    }

    [Fact]
    public async Task OpenApi_Document_ShouldContainProductsEndpoints()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/openapi/v1.json");
        var content = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(content);

        // Assert
        document.RootElement.TryGetProperty("paths", out var paths).Should().BeTrue();

        // Check for Products endpoints
        paths.TryGetProperty("/api/Products", out _).Should().BeTrue("GET and POST endpoints should be documented");
        paths.TryGetProperty("/api/Products/{id}", out _).Should().BeTrue("GET, PUT, and DELETE by ID endpoints should be documented");
    }

    [Fact]
    public async Task OpenApi_Document_ShouldContainExpectedHttpMethods()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/openapi/v1.json");
        var content = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(content);

        // Assert
        document.RootElement.TryGetProperty("paths", out var paths).Should().BeTrue();

        // Verify /api/Products has GET and POST
        if (paths.TryGetProperty("/api/Products", out var productsPath))
        {
            productsPath.TryGetProperty("get", out _).Should().BeTrue("GET /api/Products should be documented");
            productsPath.TryGetProperty("post", out _).Should().BeTrue("POST /api/Products should be documented");
        }

        // Verify /api/Products/{id} has GET, PUT, and DELETE
        if (paths.TryGetProperty("/api/Products/{id}", out var productByIdPath))
        {
            productByIdPath.TryGetProperty("get", out _).Should().BeTrue("GET /api/Products/{id} should be documented");
            productByIdPath.TryGetProperty("put", out _).Should().BeTrue("PUT /api/Products/{id} should be documented");
            productByIdPath.TryGetProperty("delete", out _).Should().BeTrue("DELETE /api/Products/{id} should be documented");
        }
    }

    [Fact]
    public async Task OpenApi_Endpoint_ShouldNotBeAccessible_InProductionEnvironment()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(Environments.Production);
        });
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/openapi/v1.json");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound, "OpenAPI endpoint should not be available in Production");
    }

    [Fact]
    public async Task OpenApi_Document_ShouldHaveValidJsonFormat()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/openapi/v1.json");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        var parseAction = () => JsonDocument.Parse(content);
        parseAction.Should().NotThrow("OpenAPI document should be valid JSON");
    }
}
