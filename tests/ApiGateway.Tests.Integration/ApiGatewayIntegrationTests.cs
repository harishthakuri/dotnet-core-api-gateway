using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace ApiGateway.Tests.Integration;

/// <summary>
/// Integration tests for the API Gateway using WebApplicationFactory
/// Tests verify middleware behavior, not downstream services
/// </summary>
public class ApiGatewayIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ApiGatewayIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task HealthEndpoint_Returns404_AsExpected()
    {
        // Health endpoint is not configured in Ocelot routes, so it returns 404
        // This is expected behavior - Ocelot only routes configured endpoints
        
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AnyRequest_IncludesCorrelationIdHeader()
    {
        // Any request through the gateway should get a correlation ID header
        
        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.Headers.Should().Contain(h => h.Key == "X-Correlation-Id");
        var correlationId = response.Headers.GetValues("X-Correlation-Id").First();
        Guid.TryParse(correlationId, out _).Should().BeTrue("correlation ID should be a valid GUID");
    }

    [Fact]
    public async Task AnyRequest_UsesProvidedCorrelationId()
    {
        // Arrange
        var correlationId = "test-integration-" + Guid.NewGuid();
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/users");
        request.Headers.Add("X-Correlation-Id", correlationId);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.Headers.Should().Contain(h => h.Key == "X-Correlation-Id");
        var responseCorrelationId = response.Headers.GetValues("X-Correlation-Id").First();
        responseCorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public async Task NotFoundRoute_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/nonexistent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
