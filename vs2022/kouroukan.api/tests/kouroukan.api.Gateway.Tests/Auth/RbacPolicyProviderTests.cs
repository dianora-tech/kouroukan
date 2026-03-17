using FluentAssertions;
using Kouroukan.Api.Gateway.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Kouroukan.Api.Gateway.Tests.Auth;

public class RbacPolicyProviderTests
{
    private readonly RbacPolicyProvider _provider;

    public RbacPolicyProviderTests()
    {
        var options = Options.Create(new AuthorizationOptions());
        _provider = new RbacPolicyProvider(options);
    }

    [Fact]
    public async Task GetPolicyAsync_PermissionPolicy_ReturnsPolicy()
    {
        // Act
        var policy = await _provider.GetPolicyAsync("RequirePermission:inscriptions:read");

        // Assert
        policy.Should().NotBeNull();
        policy!.Requirements.Should().HaveCount(2);
        policy.Requirements.OfType<PermissionRequirement>()
            .Should().ContainSingle()
            .Which.Permission.Should().Be("inscriptions:read");
    }

    [Fact]
    public async Task GetPolicyAsync_DifferentPermissions_ReturnsDifferentPolicies()
    {
        // Act
        var policy1 = await _provider.GetPolicyAsync("RequirePermission:inscriptions:read");
        var policy2 = await _provider.GetPolicyAsync("RequirePermission:finances:create");

        // Assert
        policy1.Should().NotBeNull();
        policy2.Should().NotBeNull();

        var req1 = policy1!.Requirements.OfType<PermissionRequirement>().Single();
        var req2 = policy2!.Requirements.OfType<PermissionRequirement>().Single();

        req1.Permission.Should().Be("inscriptions:read");
        req2.Permission.Should().Be("finances:create");
    }

    [Fact]
    public async Task GetPolicyAsync_SamePermissionTwice_ReturnsCachedPolicy()
    {
        // Act
        var policy1 = await _provider.GetPolicyAsync("RequirePermission:presences:read");
        var policy2 = await _provider.GetPolicyAsync("RequirePermission:presences:read");

        // Assert
        policy1.Should().BeSameAs(policy2);
    }

    [Fact]
    public async Task GetPolicyAsync_NonPermissionPolicy_DelegatesToFallback()
    {
        // Act
        var policy = await _provider.GetPolicyAsync("SomeOtherPolicy");

        // Assert - fallback returns null for unknown policies
        policy.Should().BeNull();
    }

    [Fact]
    public async Task GetDefaultPolicyAsync_ReturnsPolicy()
    {
        // Act
        var policy = await _provider.GetDefaultPolicyAsync();

        // Assert
        policy.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPolicyAsync_RequiresAuthentication()
    {
        // Act
        var policy = await _provider.GetPolicyAsync("RequirePermission:bde:read");

        // Assert - policy should require authenticated user
        policy.Should().NotBeNull();
        policy!.Requirements.OfType<Microsoft.AspNetCore.Authorization.Infrastructure.DenyAnonymousAuthorizationRequirement>()
            .Should().ContainSingle();
    }
}
