using GnCache.Domain;
using GnCache.Infrastructure.Models;
using GnCache.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GnCache.Tests;

public sealed class CacheRegistryTests
{
    private readonly List<CacheEntityRegistration> _registrations =
    [
        new CacheEntityRegistration
        {
            CacheKey = "regions",
            EntityType = typeof(Region),
            SourceApiUrl = "/api/geo/regions",
            CronExpression = "0 0 */12 * * ?",
            SeedFileName = "regions.json"
        },
        new CacheEntityRegistration
        {
            CacheKey = "matieres",
            EntityType = typeof(Matiere),
            SourceApiUrl = "/api/pedagogie/matieres",
            CronExpression = "0 0 */6 * * ?",
            SeedFileName = "matieres.json"
        }
    ];

    private CacheRegistry CreateRegistry(IServiceProvider? serviceProvider = null)
    {
        var sp = serviceProvider ?? new ServiceCollection().BuildServiceProvider();
        var logger = new Mock<ILogger<CacheRegistry>>();
        return new CacheRegistry(_registrations, sp, logger.Object);
    }

    [Fact]
    public void GetAllRegistrations_ShouldReturnAllRegistrations()
    {
        var registry = CreateRegistry();
        var result = registry.GetAllRegistrations();
        result.Should().HaveCount(2);
    }

    [Fact]
    public void GetRegistration_ShouldReturnCorrectRegistration()
    {
        var registry = CreateRegistry();
        var result = registry.GetRegistration("regions");
        result.Should().NotBeNull();
        result!.CacheKey.Should().Be("regions");
        result.EntityType.Should().Be(typeof(Region));
    }

    [Fact]
    public void GetRegistration_ShouldReturnNull_WhenKeyNotFound()
    {
        var registry = CreateRegistry();
        var result = registry.GetRegistration("unknown");
        result.Should().BeNull();
    }

    [Fact]
    public void GetRegistration_ShouldBeCaseInsensitive()
    {
        var registry = CreateRegistry();
        var result = registry.GetRegistration("REGIONS");
        result.Should().NotBeNull();
    }
}
