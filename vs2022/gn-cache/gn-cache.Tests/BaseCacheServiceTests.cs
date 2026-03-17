using GnCache.Application.Services;
using GnCache.Domain;
using GnCache.Infrastructure.Models;
using GnCache.Infrastructure.Options;
using GnCache.Infrastructure.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GnCache.Tests;

public sealed class BaseCacheServiceTests
{
    private readonly Mock<IMemoryCache> _memoryCacheMock = new();
    private readonly Mock<IDistributedCache> _distributedCacheMock = new();
    private readonly Mock<ICacheStatusService> _statusServiceMock = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
    private readonly Mock<ILogger<BaseCacheService<Region>>> _loggerMock = new();

    private readonly CacheEntityRegistration _registration = new()
    {
        CacheKey = "regions",
        EntityType = typeof(Region),
        SourceApiUrl = "/api/geo/regions",
        CronExpression = "0 0 */12 * * ?",
        SeedFileName = "regions.json"
    };

    private readonly CacheOptions _options = new()
    {
        RedisKeyPrefix = "gn-cache:",
        SeedDataPath = "data/seed"
    };

    private BaseCacheService<Region> CreateService()
    {
        return new BaseCacheService<Region>(
            _memoryCacheMock.Object,
            _distributedCacheMock.Object,
            _statusServiceMock.Object,
            _httpClientFactoryMock.Object,
            Options.Create(_options),
            _registration,
            _loggerMock.Object);
    }

    [Fact]
    public void CacheKey_ShouldReturnRegistrationKey()
    {
        var service = CreateService();
        service.CacheKey.Should().Be("regions");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnFromL1_WhenCacheExists()
    {
        // Arrange
        var expected = new List<Region>
        {
            new() { Id = 1, Name = "Conakry", Code = "CKY" }
        }.AsReadOnly();

        object? cacheValue = expected;
        _memoryCacheMock
            .Setup(x => x.TryGetValue("gn-cache:regions", out cacheValue))
            .Returns(true);

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync();

        // Assert
        result.Should().BeSameAs(expected);
        _statusServiceMock.Verify(x => x.RecordL1Hit("regions"), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldPromoteToL1_WhenFoundInL2()
    {
        // Arrange
        object? cacheValue = null;
        _memoryCacheMock
            .Setup(x => x.TryGetValue("gn-cache:regions", out cacheValue))
            .Returns(false);

        var regions = new List<Region>
        {
            new() { Id = 1, Name = "Conakry", Code = "CKY" }
        };
        var json = JsonSerializer.Serialize(regions);

        _distributedCacheMock
            .Setup(x => x.GetAsync("gn-cache:regions", It.IsAny<CancellationToken>()))
            .ReturnsAsync(System.Text.Encoding.UTF8.GetBytes(json));

        var cacheEntry = Mock.Of<ICacheEntry>();
        _memoryCacheMock
            .Setup(x => x.CreateEntry("gn-cache:regions"))
            .Returns(cacheEntry);

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        _statusServiceMock.Verify(x => x.RecordL2Hit("regions"), Times.Once);
    }

    [Fact]
    public async Task InvalidateAsync_ShouldRemoveFromBothLevels()
    {
        // Arrange
        var service = CreateService();

        // Act
        await service.InvalidateAsync();

        // Assert
        _memoryCacheMock.Verify(x => x.Remove("gn-cache:regions"), Times.Once);
        _distributedCacheMock.Verify(
            x => x.RemoveAsync("gn-cache:regions", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task InvalidateAsync_ShouldNotThrow_WhenRedisFails()
    {
        // Arrange
        _distributedCacheMock
            .Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Redis down"));

        var service = CreateService();

        // Act
        var act = () => service.InvalidateAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }
}
