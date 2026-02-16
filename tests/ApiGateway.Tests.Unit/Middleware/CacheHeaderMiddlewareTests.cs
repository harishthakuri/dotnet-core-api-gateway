using ApiGateway.Api.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace ApiGateway.Tests.Unit.Middleware;

public class CacheHeaderMiddlewareTests
{
    private readonly Mock<ILogger<CacheHeaderMiddleware>> _loggerMock;
    private readonly Mock<RequestDelegate> _nextMock;

    public CacheHeaderMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<CacheHeaderMiddleware>>();
        _nextMock = new Mock<RequestDelegate>();
    }

    [Fact]
    public async Task InvokeAsync_SuccessfulRequest_AddsResponseTimeHeader()
    {
        // Arrange
        var middleware = new CacheHeaderMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Response.StatusCode = 200;

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Headers["X-Response-Time"].Should().NotBeEmpty();
        var responseTime = context.Response.Headers["X-Response-Time"].ToString();
        responseTime.Should().EndWith("ms");
    }

    [Fact]
    public async Task InvokeAsync_SuccessfulRequest_AddsCacheStatusHeader()
    {
        // Arrange
        var middleware = new CacheHeaderMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Response.StatusCode = 200;

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Headers["X-Cache-Status"].Should().NotBeEmpty();
        var cacheStatus = context.Response.Headers["X-Cache-Status"].ToString();
        cacheStatus.Should().BeOneOf("HIT", "MISS");
    }

    [Fact]
    public async Task InvokeAsync_NonSuccessRequest_DoesNotAddHeaders()
    {
        // Arrange
        var middleware = new CacheHeaderMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Response.StatusCode = 404;

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Headers.ContainsKey("X-Cache-Status").Should().BeFalse();
        context.Response.Headers.ContainsKey("X-Response-Time").Should().BeFalse();
    }

    [Fact]
    public async Task InvokeAsync_CopiesResponseBodyCorrectly()
    {
        // Arrange
        var middleware = new CacheHeaderMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext();
        var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        context.Response.StatusCode = 200;

        var testContent = "test response content";
        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .Callback<HttpContext>(ctx =>
            {
                var bytes = Encoding.UTF8.GetBytes(testContent);
                ctx.Response.Body.Write(bytes, 0, bytes.Length);
                ctx.Response.Body.Seek(0, SeekOrigin.Begin);
            })
            .Returns(Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        responseBody.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(responseBody);
        var actualContent = await reader.ReadToEndAsync();
        actualContent.Should().Be(testContent);
    }
}
