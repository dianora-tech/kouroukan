using FluentAssertions;
using GnDapper.Connection;
using GnSecurity.Rbac;
using Kouroukan.Api.Gateway.Auth;
using Moq;

namespace Kouroukan.Api.Gateway.Tests.Auth;

public sealed class PermissionStoreTests
{
    private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;
    private readonly PermissionStore _sut;

    public PermissionStoreTests()
    {
        _connectionFactoryMock = new Mock<IDbConnectionFactory>();
        _sut = new PermissionStore(_connectionFactoryMock.Object);
    }

    // ─── Construction ───

    [Fact]
    public void Constructeur_DoitCreerInstance_QuandDependanceValide()
    {
        // Assert
        _sut.Should().NotBeNull();
    }

    [Fact]
    public void Constructeur_DoitImplementerIPermissionStore()
    {
        // Assert
        _sut.Should().BeAssignableTo<IPermissionStore>();
    }

    // ─── Interface ───

    [Fact]
    public void Interface_DoitExposerGetRolesForUserAsync()
    {
        var method = typeof(IPermissionStore).GetMethod("GetRolesForUserAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task<IReadOnlyList<string>>));
    }

    [Fact]
    public void Interface_DoitExposerGetPermissionsForUserAsync()
    {
        var method = typeof(IPermissionStore).GetMethod("GetPermissionsForUserAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task<IReadOnlyList<string>>));
    }

    [Fact]
    public void Interface_DoitExposerAssignRoleAsync()
    {
        var method = typeof(IPermissionStore).GetMethod("AssignRoleAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task));
    }

    [Fact]
    public void Interface_DoitExposerRevokeRoleAsync()
    {
        var method = typeof(IPermissionStore).GetMethod("RevokeRoleAsync");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task));
    }
}
