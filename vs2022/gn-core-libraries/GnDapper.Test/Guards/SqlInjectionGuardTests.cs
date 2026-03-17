using GnDapper.Exceptions;
using GnDapper.Guards;

namespace GnDapper.Test.Guards;

public sealed class SqlInjectionGuardTests
{
    // ========================================================================
    // Requetes valides
    // ========================================================================

    [Fact]
    public void Validate_SafeSelectWithParameter_DoesNotThrow()
    {
        // Arrange
        var sql = "SELECT * FROM users WHERE id = @Id";

        // Act
        var act = () => SqlInjectionGuard.Validate(sql);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_SafeDeleteWithWhereClause_DoesNotThrow()
    {
        // Arrange
        var sql = "DELETE FROM users WHERE id = @Id";

        // Act
        var act = () => SqlInjectionGuard.Validate(sql);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_SafeInsertStatement_DoesNotThrow()
    {
        // Arrange
        var sql = "INSERT INTO users (name, email) VALUES (@Name, @Email) RETURNING *";

        // Act
        var act = () => SqlInjectionGuard.Validate(sql);

        // Assert
        act.Should().NotThrow();
    }

    // ========================================================================
    // Requetes dangereuses - DROP
    // ========================================================================

    [Theory]
    [InlineData("DROP TABLE users")]
    [InlineData("DROP DATABASE production")]
    [InlineData("DROP INDEX idx_users_email")]
    [InlineData("DROP SCHEMA public")]
    public void Validate_DropStatement_ThrowsDataAccessException(string sql)
    {
        // Act
        var act = () => SqlInjectionGuard.Validate(sql);

        // Assert
        act.Should().Throw<DataAccessException>()
            .WithMessage("*DROP*");
    }

    [Fact]
    public void Validate_DropTableCaseInsensitive_ThrowsDataAccessException()
    {
        // Arrange
        var sql = "drop TABLE users";

        // Act
        var act = () => SqlInjectionGuard.Validate(sql);

        // Assert
        act.Should().Throw<DataAccessException>()
            .WithMessage("*DROP*");
    }

    // ========================================================================
    // Requetes dangereuses - DELETE sans WHERE
    // ========================================================================

    [Fact]
    public void Validate_DeleteWithoutWhere_ThrowsDataAccessException()
    {
        // Arrange
        var sql = "DELETE FROM users";

        // Act
        var act = () => SqlInjectionGuard.Validate(sql);

        // Assert
        act.Should().Throw<DataAccessException>()
            .WithMessage("*DELETE sans clause WHERE*");
    }

    // ========================================================================
    // Requetes dangereuses - TRUNCATE
    // ========================================================================

    [Fact]
    public void Validate_TruncateTable_ThrowsDataAccessException()
    {
        // Arrange
        var sql = "TRUNCATE TABLE users";

        // Act
        var act = () => SqlInjectionGuard.Validate(sql);

        // Assert
        act.Should().Throw<DataAccessException>()
            .WithMessage("*TRUNCATE*");
    }

    [Fact]
    public void Validate_TruncateWithoutTableKeyword_ThrowsDataAccessException()
    {
        // Arrange
        var sql = "TRUNCATE users";

        // Act
        var act = () => SqlInjectionGuard.Validate(sql);

        // Assert
        act.Should().Throw<DataAccessException>()
            .WithMessage("*TRUNCATE*");
    }

    // ========================================================================
    // Requetes dangereuses - Commentaires SQL
    // ========================================================================

    [Fact]
    public void Validate_SqlComment_ThrowsDataAccessException()
    {
        // Arrange
        var sql = "SELECT * FROM users -- all records";

        // Act
        var act = () => SqlInjectionGuard.Validate(sql);

        // Assert
        act.Should().Throw<DataAccessException>()
            .WithMessage("*commentaires SQL*");
    }

    // ========================================================================
    // Requetes dangereuses - UNION SELECT
    // ========================================================================

    [Fact]
    public void Validate_UnionSelect_ThrowsDataAccessException()
    {
        // Arrange
        var sql = "SELECT * FROM users UNION SELECT * FROM admins";

        // Act
        var act = () => SqlInjectionGuard.Validate(sql);

        // Assert
        act.Should().Throw<DataAccessException>()
            .WithMessage("*UNION SELECT*");
    }

    [Fact]
    public void Validate_UnionAllSelect_ThrowsDataAccessException()
    {
        // Arrange
        var sql = "SELECT * FROM users UNION ALL SELECT * FROM admins";

        // Act
        var act = () => SqlInjectionGuard.Validate(sql);

        // Assert
        act.Should().Throw<DataAccessException>()
            .WithMessage("*UNION SELECT*");
    }

    // ========================================================================
    // Requetes dangereuses - xp_ commands
    // ========================================================================

    [Fact]
    public void Validate_XpCmdshell_ThrowsDataAccessException()
    {
        // Arrange
        var sql = "xp_cmdshell 'dir'";

        // Act
        var act = () => SqlInjectionGuard.Validate(sql);

        // Assert
        act.Should().Throw<DataAccessException>()
            .WithMessage("*xp_*");
    }

    // ========================================================================
    // Requetes dangereuses - EXEC
    // ========================================================================

    [Fact]
    public void Validate_ExecCommand_ThrowsDataAccessException()
    {
        // Arrange
        var sql = "EXEC('SELECT * FROM users')";

        // Act
        var act = () => SqlInjectionGuard.Validate(sql);

        // Assert
        act.Should().Throw<DataAccessException>()
            .WithMessage("*EXEC*");
    }

    // ========================================================================
    // Entrees invalides
    // ========================================================================

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_NullOrEmptyOrWhitespace_ThrowsArgumentException(string? sql)
    {
        // Act
        var act = () => SqlInjectionGuard.Validate(sql!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
