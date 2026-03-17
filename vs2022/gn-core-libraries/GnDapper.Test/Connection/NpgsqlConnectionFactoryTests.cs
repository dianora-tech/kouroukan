using GnDapper.Connection;
using GnDapper.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace GnDapper.Test.Connection;

public sealed class NpgsqlConnectionFactoryTests
{
    [Fact]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        IOptions<GnDapperOptions> options = null!;

        // Act
        var act = () => new NpgsqlConnectionFactory(options);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("options");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyOrWhitespaceConnectionString_ThrowsArgumentException(string connectionString)
    {
        // Arrange
        var gnOptions = new GnDapperOptions { ConnectionString = connectionString };
        var options = Microsoft.Extensions.Options.Options.Create(gnOptions);

        // Act
        var act = () => new NpgsqlConnectionFactory(options);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateConnection_ValidConnectionString_ReturnsNpgsqlConnection()
    {
        // Arrange
        var gnOptions = new GnDapperOptions
        {
            ConnectionString = "Host=localhost;Database=testdb;Username=user;Password=pass"
        };
        var options = Microsoft.Extensions.Options.Options.Create(gnOptions);
        var factory = new NpgsqlConnectionFactory(options);

        // Act
        using var connection = factory.CreateConnection();

        // Assert
        connection.Should().NotBeNull();
        connection.Should().BeOfType<NpgsqlConnection>();
    }
}
