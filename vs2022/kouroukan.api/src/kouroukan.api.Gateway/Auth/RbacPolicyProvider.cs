using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Kouroukan.Api.Gateway.Auth;

/// <summary>
/// Fournisseur dynamique de policies d'autorisation basees sur les permissions.
/// Parse les noms de policy "RequirePermission:{name}" et cree des policies a la volee.
/// </summary>
public sealed class RbacPolicyProvider : IAuthorizationPolicyProvider
{
    private const string PolicyPrefix = "RequirePermission:";
    private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;
    private readonly ConcurrentDictionary<string, AuthorizationPolicy> _policyCache = new();

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="RbacPolicyProvider"/>.
    /// </summary>
    public RbacPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    /// <inheritdoc />
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return _fallbackProvider.GetPolicyAsync(policyName);
        }

        var policy = _policyCache.GetOrAdd(policyName, name =>
        {
            var permissionName = name[PolicyPrefix.Length..];
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(permissionName))
                .Build();
        });

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }

    /// <inheritdoc />
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        _fallbackProvider.GetDefaultPolicyAsync();

    /// <inheritdoc />
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        _fallbackProvider.GetFallbackPolicyAsync();
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

/// <summary>
/// Handler d'autorisation qui verifie les permissions via GnSecurity RBAC.
/// </summary>
public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly GnSecurity.Rbac.IRbacService _rbacService;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="PermissionAuthorizationHandler"/>.
    /// </summary>
    public PermissionAuthorizationHandler(GnSecurity.Rbac.IRbacService rbacService)
    {
        ArgumentNullException.ThrowIfNull(rbacService);
        _rbacService = rbacService;
    }

    /// <inheritdoc />
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst("sub")?.Value
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
