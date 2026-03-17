using System.Security.Claims;
using GnDapper.Connection;
using GnDapper.Options;
using GnDapper.Repositories;
using GnDapper.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Npgsql;

namespace GnDapper.Test.Repositories;

public sealed class AuditRepositoryTests
{
    private readonly Mock<IDbConnectionFactory> _mockConnectionFactory;
    private readonly ILogger<Repository<AuditableTestEntity>> _logger;
    private readonly IOptions<GnDapperOptions> _options;

    public AuditRepositoryTests()
    {
        _mockConnectionFactory = new Mock<IDbConnectionFactory>();
        _mockConnectionFactory
            .Setup(f => f.CreateConnection())
            .Returns(() => new NpgsqlConnection("Host=localhost;Database=testdb;Username=user;Password=pass"));

        _logger = NullLogger<Repository<AuditableTestEntity>>.Instance;
        _options = Microsoft.Extensions.Options.Options.Create(new GnDapperOptions
        {
            ConnectionString = "Host=localhost;Database=testdb",
            EnableSqlInjectionGuard = true
        });
    }

    private AuditRepository<AuditableTestEntity> CreateAuditRepository(
        IHttpContextAccessor? httpContextAccessor = null)
    {
        var accessor = httpContextAccessor ?? new Mock<IHttpContextAccessor>().Object;
        return new AuditRepository<AuditableTestEntity>(
            _mockConnectionFactory.Object,
            _logger,
            _options,
            accessor);
    }

    private static Mock<IHttpContextAccessor> CreateMockHttpContextAccessor(string? userName)
    {
        var mock = new Mock<IHttpContextAccessor>();

        if (userName is not null)
        {
            var identity = new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, userName)],
                "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };
            mock.Setup(a => a.HttpContext).Returns(httpContext);
        }
        else
        {
            mock.Setup(a => a.HttpContext).Returns((HttpContext?)null);
        }

        return mock;
    }

    // ========================================================================
    // Constructor
    // ========================================================================

    [Fact]
    public void Constructor_NullHttpContextAccessor_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new AuditRepository<AuditableTestEntity>(
            _mockConnectionFactory.Object,
            _logger,
            _options,
            null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("httpContextAccessor");
    }

    // ========================================================================
    // AddAsync - Champs d'audit
    // ========================================================================

    [Fact]
    public async Task AddAsync_NoHttpContext_SetsCreatedByToSystem()
    {
        // Arrange
        var accessor = CreateMockHttpContextAccessor(null);
        var repository = CreateAuditRepository(accessor.Object);
        var entity = new AuditableTestEntity { Name = "Test" };
        var beforeCall = DateTime.UtcNow;

        // Act - Le Dapper va echouer car pas de vraie connexion, mais l'entite est modifiee avant l'appel DB
        try
        {
            await repository.AddAsync(entity);
        }
        catch (NpgsqlException)
        {
            // Attendu : pas de vraie base de donnees
        }

        // Assert - Les champs d'audit sont remplis AVANT l'appel a la base
        entity.CreatedBy.Should().Be("system");
        entity.CreatedAt.Should().BeOnOrAfter(beforeCall);
        entity.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task AddAsync_WithAuthenticatedUser_SetsCreatedByToUserName()
    {
        // Arrange
        var accessor = CreateMockHttpContextAccessor("ibrahima.doumbouya");
        var repository = CreateAuditRepository(accessor.Object);
        var entity = new AuditableTestEntity { Name = "Test" };

        // Act
        try
        {
            await repository.AddAsync(entity);
        }
        catch (NpgsqlException)
        {
            // Attendu : pas de vraie base de donnees
        }

        // Assert
        entity.CreatedBy.Should().Be("ibrahima.doumbouya");
        entity.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task AddAsync_SetsCreatedAtToUtcNow()
    {
        // Arrange
        var accessor = CreateMockHttpContextAccessor(null);
        var repository = CreateAuditRepository(accessor.Object);
        var entity = new AuditableTestEntity { Name = "Test" };
        var beforeCall = DateTime.UtcNow;

        // Act
        try
        {
            await repository.AddAsync(entity);
        }
        catch (NpgsqlException)
        {
            // Attendu
        }

        // Assert
        entity.CreatedAt.Should().BeOnOrAfter(beforeCall);
        entity.CreatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public async Task AddAsync_SetsIsDeletedToFalse()
    {
        // Arrange
        var accessor = CreateMockHttpContextAccessor(null);
        var repository = CreateAuditRepository(accessor.Object);
        var entity = new AuditableTestEntity { Name = "Test", IsDeleted = true };

        // Act
        try
        {
            await repository.AddAsync(entity);
        }
        catch (NpgsqlException)
        {
            // Attendu
        }

        // Assert
        entity.IsDeleted.Should().BeFalse();
    }

    // ========================================================================
    // UpdateAsync - Champs d'audit
    // ========================================================================

    [Fact]
    public async Task UpdateAsync_NoHttpContext_SetsUpdatedByToSystem()
    {
        // Arrange
        var accessor = CreateMockHttpContextAccessor(null);
        var repository = CreateAuditRepository(accessor.Object);
        var entity = new AuditableTestEntity
        {
            Id = 1,
            Name = "Test",
            CreatedBy = "original",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        var beforeCall = DateTime.UtcNow;

        // Act
        try
        {
            await repository.UpdateAsync(entity);
        }
        catch (NpgsqlException)
        {
            // Attendu
        }

        // Assert
        entity.UpdatedBy.Should().Be("system");
        entity.UpdatedAt.Should().NotBeNull();
        entity.UpdatedAt!.Value.Should().BeOnOrAfter(beforeCall);
    }

    [Fact]
    public async Task UpdateAsync_WithAuthenticatedUser_SetsUpdatedByToUserName()
    {
        // Arrange
        var accessor = CreateMockHttpContextAccessor("admin.user");
        var repository = CreateAuditRepository(accessor.Object);
        var entity = new AuditableTestEntity
        {
            Id = 1,
            Name = "Test",
            CreatedBy = "original",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        try
        {
            await repository.UpdateAsync(entity);
        }
        catch (NpgsqlException)
        {
            // Attendu
        }

        // Assert
        entity.UpdatedBy.Should().Be("admin.user");
    }

    [Fact]
    public async Task UpdateAsync_SetsUpdatedAtToUtcNow()
    {
        // Arrange
        var accessor = CreateMockHttpContextAccessor(null);
        var repository = CreateAuditRepository(accessor.Object);
        var entity = new AuditableTestEntity
        {
            Id = 1,
            Name = "Test",
            CreatedBy = "original",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        var beforeCall = DateTime.UtcNow;

        // Act
        try
        {
            await repository.UpdateAsync(entity);
        }
        catch (NpgsqlException)
        {
            // Attendu
        }

        // Assert
        entity.UpdatedAt.Should().NotBeNull();
        entity.UpdatedAt!.Value.Should().BeOnOrAfter(beforeCall);
        entity.UpdatedAt!.Value.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    // ========================================================================
    // AddAsync - Null entity
    // ========================================================================

    [Fact]
    public async Task AddAsync_NullEntity_ThrowsArgumentNullException()
    {
        // Arrange
        var repository = CreateAuditRepository();

        // Act
        var act = () => repository.AddAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateAsync_NullEntity_ThrowsArgumentNullException()
    {
        // Arrange
        var repository = CreateAuditRepository();

        // Act
        var act = () => repository.UpdateAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
