using ApiGateway.Api.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiGateway.Tests.Unit.Middleware;

public class CorrelationIdMiddlewareTests
{
    private readonly Mock<ILogger<CorrelationIdMiddleware>> _loggerMock;
    private readonly Mock<RequestDelegate> _nextMock;

    public CorrelationIdMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<CorrelationIdMiddleware>>();
        _nextMock = new Mock<RequestDelegate>();
    }

    [Fact]
    public async Task InvokeAsync_WithExistingCorrelationId_UsesProvidedId()
    {
        // Arrange
        var middleware = new CorrelationIdMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext();
        var correlationId = "test-correlation-123";
        context.Request.Headers["X-Correlation-Id"] = correlationId;

        // Act
        await middleware.InvokeAsync(context);

        // Assert - verify the logging scope contains the provided correlation ID
        _loggerMock.Verify(
            logger => logger.BeginScope(
                It.Is<Dictionary<string, object>>(d => 
                    d.ContainsKey("X-Correlation-Id") && 
                    d["X-Correlation-Id"].ToString() == correlationId)),
            Times.Once,
            "Middleware should use the provided correlation ID in logging scope");
            
        _nextMock.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithoutCorrelationId_GeneratesNewId()
    {
        // Arrange
        var middleware = new CorrelationIdMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext();
        string? capturedId = null;

        _loggerMock.Setup(x => x.BeginScope(It.IsAny<Dictionary<string, object>>()))
            .Callback<Dictionary<string, object>>(d =>
            {
                if (d.ContainsKey("X-Correlation-Id"))
                {
                    capturedId = d["X-Correlation-Id"].ToString();
                }
            })
            .Returns((IDisposable)null!);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        capturedId.Should().NotBeNullOrEmpty();
        Guid.TryParse(capturedId, out _).Should().BeTrue("generated ID should be a valid GUID");
        _nextMock.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_CreatesLoggingScope()
    {
        // Arrange
        var middleware = new CorrelationIdMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext();
        var correlationId = "test-123";
        context.Request.Headers["X-Correlation-Id"] = correlationId;

        // Act
        await middleware.InvokeAsync(context);

        // Assert - Check that BeginScope was called with a dictionary containing CorrelationId
        _loggerMock.Verify(
            logger => logger.BeginScope(
                It.Is<Dictionary<string, object>>(d => 
                    d.ContainsKey("X-Correlation-Id") && 
                    d["X-Correlation-Id"].ToString() == correlationId)),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_CallsNextMiddleware()
    {
        // Arrange
        var middleware = new CorrelationIdMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _nextMock.Verify(next => next(context), Times.Once);
    }
}
