using FluentAssertions;
using GnSecurity.Rbac;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace GnSecurity.Test.Rbac;

/// <summary>
/// Tests unitaires pour <see cref="RbacService"/>.
/// </summary>
public sealed class RbacServiceTests
{
    private readonly Mock<IPermissionStore> _storeMock;
    private readonly IMemoryCache _cache;
    private readonly RbacService _service;

    /// <summary>ID utilisateur admin avec tous les droits.</summary>
    private const int AdminUserId = 1;

    /// <summary>ID utilisateur lecteur avec droits limites.</summary>
    private const int LecteurUserId = 2;

    /// <summary>ID utilisateur inconnu sans aucun droit.</summary>
    private const int UnknownUserId = 999;

    public RbacServiceTests()
    {
        _storeMock = new Mock<IPermissionStore>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        var logger = NullLogger<RbacService>.Instance;

        // Setup : Admin a toutes les permissions
        _storeMock
            .Setup(s => s.GetRolesForUserAsync(AdminUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "super_admin", "directeur" });

        _storeMock
            .Setup(s => s.GetPermissionsForUserAsync(AdminUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>
            {
                "inscriptions:read", "inscriptions:write", "inscriptions:delete",
                "notes:read", "notes:write",
                "finances:read", "finances:write",
                "personnel:read", "personnel:write",
                "parametres:read", "parametres:write"
            });

        // Setup : Lecteur a uniquement les permissions de lecture
        _storeMock
            .Setup(s => s.GetRolesForUserAsync(LecteurUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "lecteur" });

        _storeMock
            .Setup(s => s.GetPermissionsForUserAsync(LecteurUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>
            {
                "inscriptions:read",
                "notes:read",
                "finances:read"
            });

        // Setup : Utilisateur inconnu — aucun role ni permission
        _storeMock
            .Setup(s => s.GetRolesForUserAsync(UnknownUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());

        _storeMock
            .Setup(s => s.GetPermissionsForUserAsync(UnknownUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());

        _service = new RbacService(_storeMock.Object, _cache, logger);
    }

    // ─── Admin : toutes les permissions ─────────────────────────────────

    [Fact]
    public async Task HasPermission_Admin_ShouldHaveAllPermissions()
    {
        // Act & Assert
        (await _service.HasPermissionAsync(AdminUserId, "inscriptions:read")).Should().BeTrue();
        (await _service.HasPermissionAsync(AdminUserId, "inscriptions:write")).Should().BeTrue();
        (await _service.HasPermissionAsync(AdminUserId, "inscriptions:delete")).Should().BeTrue();
        (await _service.HasPermissionAsync(AdminUserId, "finances:write")).Should().BeTrue();
        (await _service.HasPermissionAsync(AdminUserId, "parametres:write")).Should().BeTrue();
    }

    [Fact]
    public async Task HasRole_Admin_ShouldHaveSuperAdminAndDirecteur()
    {
        // Act & Assert
        (await _service.HasRoleAsync(AdminUserId, "super_admin")).Should().BeTrue();
        (await _service.HasRoleAsync(AdminUserId, "directeur")).Should().BeTrue();
        (await _service.HasRoleAsync(AdminUserId, "lecteur")).Should().BeFalse();
    }

    // ─── Lecteur : permissions de lecture uniquement ─────────────────────

    [Fact]
    public async Task HasPermission_Lecteur_ShouldOnlyHaveReadPermissions()
    {
        // Act & Assert — lecture OK
        (await _service.HasPermissionAsync(LecteurUserId, "inscriptions:read")).Should().BeTrue();
        (await _service.HasPermissionAsync(LecteurUserId, "notes:read")).Should().BeTrue();
        (await _service.HasPermissionAsync(LecteurUserId, "finances:read")).Should().BeTrue();

        // Act & Assert — ecriture KO
        (await _service.HasPermissionAsync(LecteurUserId, "inscriptions:write")).Should().BeFalse();
        (await _service.HasPermissionAsync(LecteurUserId, "notes:write")).Should().BeFalse();
        (await _service.HasPermissionAsync(LecteurUserId, "finances:write")).Should().BeFalse();
    }

    [Fact]
    public async Task HasRole_Lecteur_ShouldOnlyHaveLecteurRole()
    {
        // Act & Assert
        (await _service.HasRoleAsync(LecteurUserId, "lecteur")).Should().BeTrue();
        (await _service.HasRoleAsync(LecteurUserId, "directeur")).Should().BeFalse();
        (await _service.HasRoleAsync(LecteurUserId, "super_admin")).Should().BeFalse();
    }

    // ─── Utilisateur inconnu : aucune permission ────────────────────────

    [Fact]
    public async Task HasPermission_UnknownUser_ShouldHaveNoPermissions()
    {
        // Act & Assert
        (await _service.HasPermissionAsync(UnknownUserId, "inscriptions:read")).Should().BeFalse();
        (await _service.HasPermissionAsync(UnknownUserId, "notes:read")).Should().BeFalse();
        (await _service.HasPermissionAsync(UnknownUserId, "parametres:write")).Should().BeFalse();
    }

    [Fact]
    public async Task HasRole_UnknownUser_ShouldHaveNoRoles()
    {
        // Act & Assert
        (await _service.HasRoleAsync(UnknownUserId, "super_admin")).Should().BeFalse();
        (await _service.HasRoleAsync(UnknownUserId, "lecteur")).Should().BeFalse();
    }

    [Fact]
    public async Task GetPermissions_UnknownUser_ShouldReturnEmptyList()
    {
        // Act
        var permissions = await _service.GetPermissionsAsync(UnknownUserId);

        // Assert
        permissions.Should().BeEmpty();
    }

    // ─── Cache ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetPermissions_ShouldBeCached_SecondCallDoesNotHitStore()
    {
        // Arrange — premier appel charge depuis le store
        await _service.GetPermissionsAsync(AdminUserId);

        // Act — deuxieme appel (doit venir du cache)
        await _service.GetPermissionsAsync(AdminUserId);

        // Assert — le store n'a ete appele qu'une seule fois
        _storeMock.Verify(
            s => s.GetPermissionsForUserAsync(AdminUserId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AssignRole_ShouldInvalidateCache()
    {
        // Arrange — charger le cache
        _storeMock.Setup(s => s.AssignRoleAsync(AdminUserId, "comptable", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _service.GetPermissionsAsync(AdminUserId);
        await _service.HasRoleAsync(AdminUserId, "super_admin");

        // Act — assigner un role (doit invalider le cache)
        await _service.AssignRoleAsync(AdminUserId, "comptable");

        // Re-charger les permissions (doit rappeler le store)
        await _service.GetPermissionsAsync(AdminUserId);

        // Assert — le store a ete appele 2 fois pour les permissions (avant et apres invalidation)
        _storeMock.Verify(
            s => s.GetPermissionsForUserAsync(AdminUserId, It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task RevokeRole_ShouldInvalidateCache()
    {
        // Arrange
        _storeMock.Setup(s => s.RevokeRoleAsync(LecteurUserId, "lecteur", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _service.HasRoleAsync(LecteurUserId, "lecteur");

        // Act
        await _service.RevokeRoleAsync(LecteurUserId, "lecteur");

        // Re-charger les roles (doit rappeler le store)
        await _service.HasRoleAsync(LecteurUserId, "lecteur");

        // Assert — le store a ete appele 2 fois pour les roles
        _storeMock.Verify(
            s => s.GetRolesForUserAsync(LecteurUserId, It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    // ─── Cas insensible a la casse ──────────────────────────────────────

    [Fact]
    public async Task HasPermission_ShouldBeCaseInsensitive()
    {
        // Act & Assert
        (await _service.HasPermissionAsync(AdminUserId, "INSCRIPTIONS:READ")).Should().BeTrue();
        (await _service.HasPermissionAsync(AdminUserId, "Inscriptions:Read")).Should().BeTrue();
    }

    [Fact]
    public async Task HasRole_ShouldBeCaseInsensitive()
    {
        // Act & Assert
        (await _service.HasRoleAsync(AdminUserId, "SUPER_ADMIN")).Should().BeTrue();
        (await _service.HasRoleAsync(AdminUserId, "Super_Admin")).Should().BeTrue();
    }
}
