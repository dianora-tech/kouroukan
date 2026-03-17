using GnSecurity.Rbac;
using Microsoft.AspNetCore.Authorization;

namespace GnCache.Api.Authorization;

/// <summary>
/// Handler d'autorisation qui verifie les permissions via GnSecurity RBAC.
/// </summary>
public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly IRbacService _rbacService;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="PermissionAuthorizationHandler"/>.
    /// </summary>
    public PermissionAuthorizationHandler(IRbacService rbacService)
    {
        ArgumentNullException.ThrowIfNull(rbacService);
        _rbacService = rbacService;
    }

    /// <inheritdoc />
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst("sub")?.Value
            ?? context.User.FindFirst("user_id")?.Value;

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
        {
            context.Fail();
            return;
        }

        if (await _rbacService.HasPermissionAsync(userId, requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}

/// <summary>
/// Requirement d'autorisation pour une permission specifique.
/// </summary>
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>Nom de la permission requise.</summary>
    public string Permission { get; }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="PermissionRequirement"/>.
    /// </summary>
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
