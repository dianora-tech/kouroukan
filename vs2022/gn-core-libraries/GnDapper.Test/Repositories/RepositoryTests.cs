using GnDapper.Connection;
using GnDapper.Exceptions;
using GnDapper.Options;
using GnDapper.Repositories;
using GnDapper.Specifications;
using GnDapper.Test.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Npgsql;

namespace GnDapper.Test.Repositories;

public sealed class RepositoryTests
{
    private readonly Mock<IDbConnectionFactory> _mockConnectionFactory;
    private readonly ILogger<Repository<SimpleTestEntity>> _logger;
    private readonly IOptions<GnDapperOptions> _options;

    public RepositoryTests()
    {
        _mockConnectionFactory = new Mock<IDbConnectionFactory>();
        _mockConnectionFactory
            .Setup(f => f.CreateConnection())
            .Returns(() => new NpgsqlConnection("Host=localhost;Database=testdb;Username=user;Password=pass"));

        _logger = NullLogger<Repository<SimpleTestEntity>>.Instance;
        _options = Microsoft.Extensions.Options.Options.Create(new GnDapperOptions
        {
            ConnectionString = "Host=localhost;Database=testdb",
            EnableSqlInjectionGuard = true
        });
    }

    private TestableRepository<T> CreateTestableRepository<T>(
        ILogger<Repository<T>>? logger = null,
        IOptions<GnDapperOptions>? options = null) where T : class, GnDapper.Entities.IEntity
    {
        var log = logger ?? NullLogger<Repository<T>>.Instance;
        var opts = options ?? _options;
        return new TestableRepository<T>(_mockConnectionFactory.Object, log, opts);
    }

    // ========================================================================
    // Metadata - Nom de table
    // ========================================================================

    [Fact]
    public void Metadata_EntityWithTableAttribute_UsesAttributeTableName()
    {
        // Arrange & Act
        var repository = CreateTestableRepository<SimpleTestEntity>();

        // Assert
        repository.GetTableName().Should().Be("test.simple_entities");
    }

    [Fact]
    public void Metadata_EntityWithoutTableAttribute_UsesSnakeCaseConvention()
    {
        // Arrange & Act
        var repository = CreateTestableRepository<NoAttributeEntity>();

        // Assert
        repository.GetTableName().Should().Be("no_attribute_entity");
    }

    // ========================================================================
    // Metadata - Colonnes INSERT
    // ========================================================================

    [Fact]
    public void Metadata_InsertColumns_ExcludesId()
    {
        // Arrange & Act
        var repository = CreateTestableRepository<SimpleTestEntity>();
        var insertColumns = repository.GetInsertPropertyNames();

        // Assert
        insertColumns.Should().NotContain("Id");
        insertColumns.Should().Contain("Name");
        insertColumns.Should().Contain("Description");
    }

    [Fact]
    public void Metadata_InsertColumnNames_AreSnakeCase()
    {
        // Arrange & Act
        var repository = CreateTestableRepository<NoAttributeEntity>();
        var insertColumnNames = repository.GetInsertColumnNames();

        // Assert
        insertColumnNames.Should().Contain("first_name");
        insertColumnNames.Should().Contain("last_name");
        insertColumnNames.Should().NotContain("id");
    }

    // ========================================================================
    // Metadata - Colonnes UPDATE
    // ========================================================================

    [Fact]
    public void Metadata_UpdateColumns_ExcludesId()
    {
        // Arrange & Act
        var repository = CreateTestableRepository<SimpleTestEntity>();
        var updateColumns = repository.GetUpdatePropertyNames();

        // Assert
        updateColumns.Should().NotContain("Id");
        updateColumns.Should().Contain("Name");
        updateColumns.Should().Contain("Description");
    }

    // ========================================================================
    // BuildSelectSql
    // ========================================================================

    [Fact]
    public void BuildSelectSql_EmptySpecification_ReturnsSelectAll()
    {
        // Arrange
        var repository = CreateTestableRepository<SimpleTestEntity>();
        var spec = new BaseSpecification<SimpleTestEntity>();

        // Act
        var sql = repository.BuildSelectSql(spec);

        // Assert
        sql.Should().Be("SELECT * FROM test.simple_entities");
    }

    [Fact]
    public void BuildSelectSql_WithWhereClause_IncludesWhere()
    {
        // Arrange
        var repository = CreateTestableRepository<SimpleTestEntity>();
        var spec = new BaseSpecification<SimpleTestEntity>()
            .Where("name = @Name", new { Name = "test" });

        // Act
        var sql = repository.BuildSelectSql(spec);

        // Assert
        sql.Should().Be("SELECT * FROM test.simple_entities WHERE name = @Name");
    }

    [Fact]
    public void BuildSelectSql_WithOrderBy_IncludesOrderBy()
    {
        // Arrange
        var repository = CreateTestableRepository<SimpleTestEntity>();
        var spec = new BaseSpecification<SimpleTestEntity>()
            .OrderByAsc("name");

        // Act
        var sql = repository.BuildSelectSql(spec);

        // Assert
        sql.Should().Be("SELECT * FROM test.simple_entities ORDER BY name ASC");
    }

    [Fact]
    public void BuildSelectSql_WithPaging_IncludesLimitAndOffset()
    {
        // Arrange
        var repository = CreateTestableRepository<SimpleTestEntity>();
        var spec = new BaseSpecification<SimpleTestEntity>()
            .WithPaging(2, 10);

        // Act
        var sql = repository.BuildSelectSql(spec);

        // Assert
        sql.Should().Be("SELECT * FROM test.simple_entities LIMIT 10 OFFSET 10");
    }

    [Fact]
    public void BuildSelectSql_WithAllClauses_IncludesAllParts()
    {
        // Arrange
        var repository = CreateTestableRepository<SimpleTestEntity>();
        var spec = new BaseSpecification<SimpleTestEntity>()
            .Where("name ILIKE @Name", new { Name = "%test%" })
            .OrderByDesc("id")
            .WithPaging(1, 20);

        // Act
        var sql = repository.BuildSelectSql(spec);

        // Assert
        sql.Should().Be("SELECT * FROM test.simple_entities WHERE name ILIKE @Name ORDER BY id DESC LIMIT 20 OFFSET 0");
    }

    // ========================================================================
    // BuildCountSql
    // ========================================================================

    [Fact]
    public void BuildCountSql_EmptySpecification_ReturnsCountAll()
    {
        // Arrange
        var repository = CreateTestableRepository<SimpleTestEntity>();
        var spec = new BaseSpecification<SimpleTestEntity>();

        // Act
        var sql = repository.BuildCountSql(spec);

        // Assert
        sql.Should().Be("SELECT COUNT(*) FROM test.simple_entities");
    }

    [Fact]
    public void BuildCountSql_WithWhereClause_IncludesWhere()
    {
        // Arrange
        var repository = CreateTestableRepository<SimpleTestEntity>();
        var spec = new BaseSpecification<SimpleTestEntity>()
            .Where("name = @Name", new { Name = "test" });

        // Act
        var sql = repository.BuildCountSql(spec);

        // Assert
        sql.Should().Be("SELECT COUNT(*) FROM test.simple_entities WHERE name = @Name");
    }

    // ========================================================================
    // SqlInjectionGuard - integration via GetWithQueryAsync
    // ========================================================================

    [Fact]
    public async Task GetWithQueryAsync_DangerousSql_ThrowsDataAccessException()
    {
        // Arrange
        var repository = CreateTestableRepository<SimpleTestEntity>();
        var dangerousSql = "SELECT * FROM users; DROP TABLE users";

        // Act
        var act = () => repository.GetWithQueryAsync(dangerousSql);

        // Assert
        await act.Should().ThrowAsync<DataAccessException>();
    }

    [Fact]
    public async Task GetWithQueryAsync_NullSql_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateTestableRepository<SimpleTestEntity>();

        // Act
        var act = () => repository.GetWithQueryAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetSingleWithQueryAsync_DangerousSql_ThrowsDataAccessException()
    {
        // Arrange
        var repository = CreateTestableRepository<SimpleTestEntity>();
        var dangerousSql = "SELECT * FROM users UNION SELECT * FROM admins";

        // Act
        var act = () => repository.GetSingleWithQueryAsync(dangerousSql);

        // Assert
        await act.Should().ThrowAsync<DataAccessException>();
    }

    // ========================================================================
    // Metadata - Entity AuditableTestEntity (table avec attribut)
    // ========================================================================

    [Fact]
    public void Metadata_AuditableEntity_UsesAttributeTableName()
    {
        // Arrange & Act
        var repository = CreateTestableRepository<AuditableTestEntity>();

        // Assert
        repository.GetTableName().Should().Be("test.auditable_entities");
    }

    [Fact]
    public void Metadata_AuditableEntity_InsertColumnsExcludeId()
    {
        // Arrange & Act
        var repository = CreateTestableRepository<AuditableTestEntity>();
        var insertPropertyNames = repository.GetInsertPropertyNames();

        // Assert
        insertPropertyNames.Should().NotContain("Id");
        insertPropertyNames.Should().Contain("Name");
        insertPropertyNames.Should().Contain("CreatedAt");
        insertPropertyNames.Should().Contain("CreatedBy");
        insertPropertyNames.Should().Contain("IsDeleted");
    }
}
