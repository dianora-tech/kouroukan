using GnCache.Domain;
using GnCache.Infrastructure.Services;

namespace GnCache.Tests;

public sealed class CacheStatusServiceTests
{
    [Fact]
    public async Task RecordL1Hit_ShouldIncrementCounter()
    {
        // Arrange
        var service = new CacheStatusService();

        // Act
        service.RecordL1Hit("regions");
        service.RecordL1Hit("regions");
        service.RecordL1Hit("regions");

        // Assert
        var status = await service.GetStatusAsync("regions");
        status.Should().NotBeNull();
        status!.L1Hits.Should().Be(3);
    }

    [Fact]
    public async Task RecordL2Hit_ShouldIncrementCounter()
    {
        // Arrange
        var service = new CacheStatusService();

        // Act
        service.RecordL2Hit("regions");

        // Assert
        var status = await service.GetStatusAsync("regions");
        status.Should().NotBeNull();
        status!.L2Hits.Should().Be(1);
    }

    [Fact]
    public async Task RecordMiss_ShouldIncrementCounter()
    {
        // Arrange
        var service = new CacheStatusService();

        // Act
        service.RecordMiss("regions");
        service.RecordMiss("regions");

        // Assert
        var status = await service.GetStatusAsync("regions");
        status!.Misses.Should().Be(2);
    }

    [Fact]
    public async Task RecordReload_ShouldUpdateMetadata()
    {
        // Arrange
        var service = new CacheStatusService();

        // Act
        service.RecordReload("regions", 8, CacheSource.Api);

        // Assert
        var status = await service.GetStatusAsync("regions");
        status!.ItemCount.Should().Be(8);
        status.LastSource.Should().Be(CacheSource.Api);
        status.LastRefreshedAtUtc.Should().NotBeNull();
        status.IsRefreshing.Should().BeFalse();
    }

    [Fact]
    public async Task RecordError_ShouldStoreErrorMessage()
    {
        // Arrange
        var service = new CacheStatusService();

        // Act
        service.RecordError("regions", "Connection refused");

        // Assert
        var status = await service.GetStatusAsync("regions");
        status!.LastError.Should().Be("Connection refused");
    }

    [Fact]
    public async Task GetAllStatusAsync_ShouldReturnAllTrackedCaches()
    {
        // Arrange
        var service = new CacheStatusService();
        service.RecordL1Hit("regions");
        service.RecordL1Hit("matieres");

        // Act
        var result = await service.GetAllStatusAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetStatusAsync_ShouldReturnNull_WhenKeyNotTracked()
    {
        // Arrange
        var service = new CacheStatusService();

        // Act
        var result = await service.GetStatusAsync("unknown");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task HitRatePercent_ShouldCalculateCorrectly()
    {
        // Arrange
        var service = new CacheStatusService();
        service.RecordL1Hit("regions"); // 1 hit
        service.RecordL1Hit("regions"); // 2 hits
        service.RecordL2Hit("regions"); // 3 total hits
        service.RecordMiss("regions");  // 1 miss => 4 total, 75% hit rate

        // Act
        var status = await service.GetStatusAsync("regions");

        // Assert
        status!.HitRatePercent.Should().Be(75);
    }
}
