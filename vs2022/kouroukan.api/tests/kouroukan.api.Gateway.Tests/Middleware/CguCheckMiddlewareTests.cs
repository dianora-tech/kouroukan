using FluentAssertions;
using Kouroukan.Api.Gateway.Auth;
using Kouroukan.Api.Gateway.Middleware;
using Kouroukan.Api.Gateway.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Text;

namespace Kouroukan.Api.Gateway.Tests.Middleware;

public class CguCheckMiddlewareTests
{
    private readonly Mock<ILogger<CguCheckMiddleware>> _loggerMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly Mock<ITokenService> _tokenServiceMock;

    public CguCheckMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<CguCheckMiddleware>>();
        _cacheMock = new Mock<IDistributedCache>();
        _tokenServiceMock = new Mock<ITokenService>();
    }

    [Fact]
    public async Task InvokeAsync_UnauthenticatedUser_PassesThrough()
    {
        // Arrange
        var nextCalled = false;
        var middleware = new CguCheckMiddleware(_ => { nextCalled = true; return Task.CompletedTask; }, _loggerMock.Object);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context, _cacheMock.Object, _tokenServiceMock.Object);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_ExcludedPath_PassesThrough()
    {
        // Arrange
        var nextCalled = false;
        var middleware = new CguCheckMiddleware(_ => { nextCalled = true; return Task.CompletedTask; }, _loggerMock.Object);
        var context = CreateAuthenticatedContext("/api/auth/login", "1.0");

        // Act
        await middleware.InvokeAsync(context, _cacheMock.Object, _tokenServiceMock.Object);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_CguVersionMatch_PassesThrough()
    {
        // Arrange
        var nextCalled = false;
        var middleware = new CguCheckMiddleware(_ => { nextCalled = true; return Task.CompletedTask; }, _loggerMock.Object);
        var context = CreateAuthenticatedContext("/api/inscriptions/eleves", "1.0");

        _cacheMock.Setup(x => x.GetAsync("cgu:active_version", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes("1.0"));

        // Act
        await middleware.InvokeAsync(context, _cacheMock.Object, _tokenServiceMock.Object);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_CguVersionMismatch_Returns403()
    {
        // Arrange
        var middleware = new CguCheckMiddleware(_ => Task.CompletedTask, _loggerMock.Object);
        var context = CreateAuthenticatedContext("/api/inscriptions/eleves", "0.9");
        context.Response.Body = new MemoryStream();

        _cacheMock.Setup(x => x.GetAsync("cgu:active_version", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes("1.0"));

        // Act
        await middleware.InvokeAsync(context, _cacheMock.Object, _tokenServiceMock.Object);

        // Assert
        context.Response.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task InvokeAsync_NoCguConfigured_PassesThrough()
    {
        // Arrange
        var nextCalled = false;
        var middleware = new CguCheckMiddleware(_ => { nextCalled = true; return Task.CompletedTask; }, _loggerMock.Object);
        var context = CreateAuthenticatedContext("/api/inscriptions/eleves", null);

        _cacheMock.Setup(x => x.GetAsync("cgu:active_version", It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        _tokenServiceMock.Setup(x => x.GetActiveCguAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CguVersionDto?)null);

        // Act
        await middleware.InvokeAsync(context, _cacheMock.Object, _tokenServiceMock.Object);

        // Assert
        nextCalled.Should().BeTrue();
    }

    private static DefaultHttpContext CreateAuthenticatedContext(string path, string? cguVersion)
    {
        var claims = new List<Claim>
        {
            new("sub", "1"),
            new("email", "test@kouroukan.gn")
        };

        if (cguVersion is not null)
            claims.Add(new Claim("cguVersion", cguVersion));

        var identity = new ClaimsIdentity(claims, "Bearer");
        var principal = new ClaimsPrincipal(identity);

        var context = new DefaultHttpContext { User = principal };
        context.Request.Path = path;
        return context;
    }
}
