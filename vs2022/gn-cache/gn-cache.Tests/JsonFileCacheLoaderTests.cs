using GnCache.Domain;
using GnCache.Infrastructure.Models;
using GnCache.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GnCache.Tests;

public sealed class JsonFileCacheLoaderTests
{
    private readonly Mock<ILogger<JsonFileCacheLoader>> _loggerMock = new();

    private readonly List<CacheEntityRegistration> _registrations =
    [
        new CacheEntityRegistration
        {
            CacheKey = "regions",
            EntityType = typeof(Region),
            SourceApiUrl = "/api/geo/regions",
            CronExpression = "0 0 */12 * * ?",
            SeedFileName = "regions.json"
        }
    ];

    [Fact]
    public async Task LoadAllSeedsAsync_ShouldNotThrow_WhenServiceNotRegistered()
    {
        // Arrange
        var sp = new ServiceCollection().BuildServiceProvider();
        var loader = new JsonFileCacheLoader(_registrations, sp, _loggerMock.Object);

        // Act
        var act = () => loader.LoadAllSeedsAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenRegistrationsNull()
    {
        // Act
        var act = () => new JsonFileCacheLoader(
            null!,
            new ServiceCollection().BuildServiceProvider(),
            _loggerMock.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
