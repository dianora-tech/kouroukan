using Microsoft.AspNetCore.Authorization;

namespace Support.Api.Authorization;

/// <summary>
/// Handler d'autorisation verifiant les permissions dans les claims JWT.
/// </summary>
public sealed class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.HasClaim("permission", requirement.Permission) ||
            context.User.IsInRole("super_admin"))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
