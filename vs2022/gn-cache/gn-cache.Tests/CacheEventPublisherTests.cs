using GnCache.Infrastructure.Services;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.RabbitMq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GnCache.Tests;

public sealed class CacheEventPublisherTests
{
    private readonly Mock<IMessagePublisher> _publisherMock = new();
    private readonly Mock<ILogger<CacheEventPublisher>> _loggerMock = new();

    private readonly RabbitMqOptions _rabbitOptions = new()
    {
        ProjectSlug = "kouroukan"
    };

    private CacheEventPublisher CreateService()
    {
        return new CacheEventPublisher(
            _publisherMock.Object,
            Options.Create(_rabbitOptions),
            _loggerMock.Object);
    }

    [Fact]
    public async Task PublishInvalidationAsync_ShouldPublishCacheInvalidatedEvent()
    {
        // Arrange
        var service = CreateService();

        // Act
        await service.PublishInvalidationAsync("regions", "Test reason");

        // Assert
        _publisherMock.Verify(
            x => x.PublishAsync(
                It.Is<CacheInvalidatedEvent>(e =>
                    e.CacheKey == "regions" && e.Reason == "Test reason"),
                "kouroukan.events",
                "cache.invalidated.regions",
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishInvalidationAsync_ShouldNotThrow_WhenPublishFails()
    {
        // Arrange
        _publisherMock
            .Setup(x => x.PublishAsync(
                It.IsAny<CacheInvalidatedEvent>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("RabbitMQ down"));

        var service = CreateService();

        // Act
        var act = () => service.PublishInvalidationAsync("regions");

        // Assert
        await act.Should().NotThrowAsync();
    }
}
