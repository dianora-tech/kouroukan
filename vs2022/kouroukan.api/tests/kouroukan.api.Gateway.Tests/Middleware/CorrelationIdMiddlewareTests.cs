using FluentAssertions;
using Kouroukan.Api.Gateway.Middleware;
using Microsoft.AspNetCore.Http;

namespace Kouroukan.Api.Gateway.Tests.Middleware;

public class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_NoHeader_GeneratesCorrelationId()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Items["CorrelationId"].Should().NotBeNull();
        context.Response.Headers["X-Correlation-Id"].ToString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task InvokeAsync_WithHeader_PropagatesCorrelationId()
    {
        // Arrange
        var expectedId = "abc123";
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-Id"] = expectedId;
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Items["CorrelationId"].Should().Be(expectedId);
        context.Response.Headers["X-Correlation-Id"].ToString().Should().Be(expectedId);
    }
}
