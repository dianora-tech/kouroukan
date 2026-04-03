using FluentAssertions;
using GnDapper.Connection;
using GnSecurity.Jwt;
using Kouroukan.Api.Gateway.Auth;
using Moq;

namespace Kouroukan.Api.Gateway.Tests.Auth;

public sealed class UserClaimsProviderTests
{
    private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;
    private readonly UserClaimsProvider _sut;

    public UserClaimsProviderTests()
    {
        _connectionFactoryMock = new Mock<IDbConnectionFactory>();
        _sut = new UserClaimsProvider(_connectionFactoryMock.Object);
    }

    // ─── Construction ───

    [Fact]
    public void Constructeur_DoitCreerInstance_QuandDependanceValide()
    {
        // Assert
        _sut.Should().NotBeNull();
    }

    [Fact]
    public void Constructeur_DoitImplementerIUserClaimsProvider()
    {
        // Assert
        _sut.Should().BeAssignableTo<IUserClaimsProvider>();
    }

    // ─── Interface ───

    [Fact]
    public void Interface_DoitExposerGetUserClaimsAsync()
    {
        var method = typeof(IUserClaimsProvider).GetMethod("GetUserClaimsAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task<UserClaims?>));
    }

    // ─── UserClaims Record ───

    [Fact]
    public void UserClaims_DoitContenirTousLesChamps()
    {
        // Arrange & Act
        var claims = new UserClaims(
            1, "test@test.com", "Alpha Barry",
            new List<string> { "directeur" },
            new List<string> { "users:manage", "inscriptions:read" });

        // Assert
        claims.UserId.Should().Be(1);
        claims.Email.Should().Be("test@test.com");
        claims.FullName.Should().Be("Alpha Barry");
        claims.Roles.Should().ContainSingle().Which.Should().Be("directeur");
        claims.Permissions.Should().HaveCount(2);
    }
}
