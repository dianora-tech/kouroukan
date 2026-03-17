using GnCache.Domain;
using GnCache.Infrastructure.Models;
using GnCache.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;

namespace GnCache.Tests;

public sealed class CacheSchedulerServiceTests
{
    private readonly Mock<ISchedulerFactory> _schedulerFactoryMock = new();
    private readonly Mock<IScheduler> _schedulerMock = new();
    private readonly Mock<ILogger<CacheSchedulerService>> _loggerMock = new();

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

    public CacheSchedulerServiceTests()
    {
        _schedulerFactoryMock
            .Setup(x => x.GetScheduler(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_schedulerMock.Object);
    }

    private CacheSchedulerService CreateService()
    {
        return new CacheSchedulerService(
            _schedulerFactoryMock.Object,
            _registrations,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetSchedulesAsync_ShouldReturnScheduleForEachRegistration()
    {
        // Arrange
        _schedulerMock
            .Setup(x => x.GetTriggersOfJob(
                It.IsAny<JobKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ITrigger>());

        var service = CreateService();

        // Act
        var result = await service.GetSchedulesAsync();

        // Assert
        result.Should().HaveCount(1);
        result[0].CacheKey.Should().Be("regions");
        result[0].CronExpression.Should().Be("0 0 */12 * * ?");
    }

    [Fact]
    public async Task TriggerAsync_ShouldTriggerJob()
    {
        // Arrange
        var service = CreateService();

        // Act
        await service.TriggerAsync("regions");

        // Assert
        _schedulerMock.Verify(
            x => x.TriggerJob(
                It.Is<JobKey>(k => k.Name == "cache-refresh-regions"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
