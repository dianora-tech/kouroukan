using GnCache.Application.Services;
using GnCache.Domain;
using GnCache.Infrastructure.Models;
using GnCache.Infrastructure.Services;
using GnMessaging.Events;
using Microsoft.Extensions.Logging;

namespace GnCache.Tests;

public sealed class CacheInvalidationHandlerTests
{
    private readonly Mock<ICacheRegistry> _registryMock = new();
    private readonly Mock<ILogger<CacheInvalidationHandler>> _loggerMock = new();

    private CacheInvalidationHandler CreateHandler()
    {
        return new CacheInvalidationHandler(
            _registryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReloadCacheOnEvent()
    {
        // Arrange
        var registration = new CacheEntityRegistration
        {
            CacheKey = "regions",
            EntityType = typeof(Region),
            SourceApiUrl = "/api/geo/regions",
            CronExpression = "0 0 */12 * * ?",
            SeedFileName = "regions.json"
        };

        _registryMock
            .Setup(x => x.GetRegistration("regions"))
            .Returns(registration);

        var handler = CreateHandler();
        var @event = new CacheInvalidatedEvent("regions", "Entity updated");

        // Act
        await handler.HandleAsync(@event);

        // Assert
        _registryMock.Verify(
            x => x.ReloadAsync("regions", CacheSource.EventDriven,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotReload_WhenCacheKeyNotFound()
    {
        // Arrange
        _registryMock
            .Setup(x => x.GetRegistration("unknown"))
            .Returns((CacheEntityRegistration?)null);

        var handler = CreateHandler();
        var @event = new CacheInvalidatedEvent("unknown");

        // Act
        await handler.HandleAsync(@event);

        // Assert
        _registryMock.Verify(
            x => x.ReloadAsync(It.IsAny<string>(), It.IsAny<CacheSource>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
