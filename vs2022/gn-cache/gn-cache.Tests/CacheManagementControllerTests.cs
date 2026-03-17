using GnCache.Api.Controllers;
using GnCache.Application.Services;
using GnCache.Domain;
using GnCache.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GnCache.Tests;

public sealed class CacheManagementControllerTests
{
    private readonly Mock<ICacheRegistry> _registryMock = new();
    private readonly Mock<ICacheStatusService> _statusServiceMock = new();
    private readonly Mock<ICacheEventPublisher> _eventPublisherMock = new();
    private readonly Mock<ILogger<CacheManagementController>> _loggerMock = new();

    private CacheManagementController CreateController()
    {
        return new CacheManagementController(
            _registryMock.Object,
            _statusServiceMock.Object,
            _eventPublisherMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetStatus_ShouldReturnOk()
    {
        // Arrange
        var statuses = new List<CacheStatistics>
        {
            new() { CacheKey = "regions", ItemCount = 8 }
        }.AsReadOnly();

        _statusServiceMock
            .Setup(x => x.GetAllStatusAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(statuses);

        var controller = CreateController();

        // Act
        var result = await controller.GetStatus(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetCache_ShouldReturnNotFound_WhenKeyInvalid()
    {
        // Arrange
        _registryMock
            .Setup(x => x.GetRegistration("unknown"))
            .Returns((CacheEntityRegistration?)null);

        var controller = CreateController();

        // Act
        var result = await controller.GetCache("unknown", CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetCache_ShouldReturnOk_WhenKeyValid()
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

        var regions = new List<Region>
        {
            new() { Id = 1, Name = "Conakry", Code = "CKY" }
        };

        _registryMock
            .Setup(x => x.GetDataAsync("regions", It.IsAny<CancellationToken>()))
            .ReturnsAsync(regions);

        var controller = CreateController();

        // Act
        var result = await controller.GetCache("regions", CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ReloadCache_ShouldReturnNotFound_WhenKeyInvalid()
    {
        // Arrange
        _registryMock
            .Setup(x => x.GetRegistration("unknown"))
            .Returns((CacheEntityRegistration?)null);

        var controller = CreateController();

        // Act
        var result = await controller.ReloadCache("unknown", CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
