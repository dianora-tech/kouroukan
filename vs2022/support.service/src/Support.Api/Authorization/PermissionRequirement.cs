using Microsoft.AspNetCore.Authorization;

namespace Support.Api.Authorization;

/// <summary>
/// Requirement pour les permissions RBAC dynamiques.
/// </summary>
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    public PermissionRequirement(string permission) => Permission = permission;
}
