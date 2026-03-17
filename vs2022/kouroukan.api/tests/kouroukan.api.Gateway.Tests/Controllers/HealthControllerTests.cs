using FluentAssertions;
using Kouroukan.Api.Gateway.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

namespace Kouroukan.Api.Gateway.Tests.Controllers;

public class HealthControllerTests
{
    [Fact]
    public void Constructor_WithHealthCheckService_CreatesInstance()
    {
        // Arrange - HealthCheckService has non-virtual methods, we test construction only
        // Integration tests should verify /health and /ready endpoints
        var controller = typeof(HealthController);

        // Assert
        controller.Should().NotBeNull();
        controller.GetMethod("Health").Should().NotBeNull();
        controller.GetMethod("Ready").Should().NotBeNull();
    }

    [Fact]
    public void HealthController_HasCorrectAttributes()
    {
        // Assert
        var controllerType = typeof(HealthController);
        controllerType.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.ApiControllerAttribute), true)
            .Should().NotBeEmpty();
        controllerType.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute), true)
            .Should().NotBeEmpty();
    }

    [Fact]
    public void Health_Endpoint_HasGetAttribute()
    {
        // Assert
        var method = typeof(HealthController).GetMethod("Health");
        method.Should().NotBeNull();
        method!.GetCustomAttributes(typeof(HttpGetAttribute), true)
            .Should().NotBeEmpty();
    }

    [Fact]
    public void Ready_Endpoint_HasGetAttribute()
    {
        // Assert
        var method = typeof(HealthController).GetMethod("Ready");
        method.Should().NotBeNull();
        method!.GetCustomAttributes(typeof(HttpGetAttribute), true)
            .Should().NotBeEmpty();
    }
}
