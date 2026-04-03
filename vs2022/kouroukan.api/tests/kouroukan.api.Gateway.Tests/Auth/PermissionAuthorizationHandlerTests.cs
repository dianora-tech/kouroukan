using FluentAssertions;
using GnSecurity.Rbac;
using Kouroukan.Api.Gateway.Auth;
using Microsoft.AspNetCore.Authorization;
using Moq;
using System.Security.Claims;

namespace Kouroukan.Api.Gateway.Tests.Auth;

public sealed class PermissionAuthorizationHandlerTests
{
    private readonly Mock<IRbacService> _rbacServiceMock;
    private readonly PermissionAuthorizationHandler _sut;

    public PermissionAuthorizationHandlerTests()
    {
        _rbacServiceMock = new Mock<IRbacService>();
        _sut = new PermissionAuthorizationHandler(_rbacServiceMock.Object);
    }

    // ─── Construction ───

    [Fact]
    public void Constructeur_DoitCreerInstance_QuandDependanceValide()
    {
        // Assert
        _sut.Should().NotBeNull();
    }

    [Fact]
    public void Constructeur_DoitLeverException_QuandRbacServiceNull()
    {
        // Act
        var act = () => new PermissionAuthorizationHandler(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructeur_DoitImplementerAuthorizationHandler()
    {
        // Assert
        _sut.Should().BeAssignableTo<IAuthorizationHandler>();
    }

    // ─── HandleRequirementAsync ───

    [Fact]
    public async Task HandleRequirementAsync_DoitReussir_QuandUtilisateurALaPermission()
    {
        // Arrange
        var requirement = new PermissionRequirement("users:manage");
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "42") };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var context = new AuthorizationHandlerContext(
            new[] { requirement }, principal, null);

        _rbacServiceMock
            .Setup(x => x.HasPermissionAsync(42, "users:manage", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await ((IAuthorizationHandler)_sut).HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_DoitEchouer_QuandUtilisateurNaPasLaPermission()
    {
        // Arrange
        var requirement = new PermissionRequirement("admin:manage");
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "42") };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var context = new AuthorizationHandlerContext(
            new[] { requirement }, principal, null);

        _rbacServiceMock
            .Setup(x => x.HasPermissionAsync(42, "admin:manage", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await ((IAuthorizationHandler)_sut).HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_DoitEchouer_QuandPasDeClaimNameIdentifier()
    {
        // Arrange
        var requirement = new PermissionRequirement("users:manage");
        var identity = new ClaimsIdentity(); // No claims
        var principal = new ClaimsPrincipal(identity);
        var context = new AuthorizationHandlerContext(
            new[] { requirement }, principal, null);

        // Act
        await ((IAuthorizationHandler)_sut).HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
        context.HasFailed.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_DoitEchouer_QuandUserIdNonNumerique()
    {
        // Arrange
        var requirement = new PermissionRequirement("users:manage");
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "not-a-number") };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var context = new AuthorizationHandlerContext(
            new[] { requirement }, principal, null);

        // Act
        await ((IAuthorizationHandler)_sut).HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
        context.HasFailed.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_DoitUtiliserClaimSub_QuandNameIdentifierAbsent()
    {
        // Arrange
        var requirement = new PermissionRequirement("users:manage");
        var claims = new[] { new Claim("sub", "42") };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var context = new AuthorizationHandlerContext(
            new[] { requirement }, principal, null);

        _rbacServiceMock
            .Setup(x => x.HasPermissionAsync(42, "users:manage", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await ((IAuthorizationHandler)_sut).HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_DoitUtiliserClaimUserId_QuandAutresAbsents()
    {
        // Arrange
        var requirement = new PermissionRequirement("inscriptions:read");
        var claims = new[] { new Claim("user_id", "99") };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var context = new AuthorizationHandlerContext(
            new[] { requirement }, principal, null);

        _rbacServiceMock
            .Setup(x => x.HasPermissionAsync(99, "inscriptions:read", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await ((IAuthorizationHandler)_sut).HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    // ─── PermissionRequirement ───

    [Fact]
    public void PermissionRequirement_DoitContenirPermission()
    {
        // Arrange & Act
        var requirement = new PermissionRequirement("inscriptions:read");

        // Assert
        requirement.Permission.Should().Be("inscriptions:read");
    }

    [Fact]
    public void PermissionRequirement_DoitImplementerIAuthorizationRequirement()
    {
        // Arrange & Act
        var requirement = new PermissionRequirement("test");

        // Assert
        requirement.Should().BeAssignableTo<IAuthorizationRequirement>();
    }
}
