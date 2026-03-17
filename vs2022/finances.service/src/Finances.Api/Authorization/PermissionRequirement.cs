using Microsoft.AspNetCore.Authorization;

namespace Finances.Api.Authorization;

/// <summary>
/// Requirement d'autorisation basee sur les permissions RBAC.
/// </summary>
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    public PermissionRequirement(string permission) => Permission = permission;
}
