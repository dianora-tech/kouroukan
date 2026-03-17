using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using GnSecurity.Jwt;
using Microsoft.Extensions.Options;

namespace GnSecurity.Test.Jwt;

/// <summary>
/// Tests unitaires pour <see cref="JwtTokenService"/>.
/// </summary>
public sealed class JwtTokenServiceTests
{
    /// <summary>Cle HMAC-SHA512 de test (minimum 64 caracteres).</summary>
    private const string TestKey =
        "CeciEstUneCleDeTestTresLonguePourHmacSha512QuiDoitFaireAuMoins64OctetsMinimum!!";

    private const string TestIssuer = "kouroukan-test";
    private const string TestAudience = "kouroukan-api-test";

    private static readonly string[] TestRoles = ["directeur", "enseignant"];
    private static readonly string[] TestPermissions = ["inscriptions:read", "inscriptions:write", "notes:read"];

    private readonly JwtTokenService _service;

    public JwtTokenServiceTests()
    {
        var options = Options.Create(new JwtOptions
        {
            Key = TestKey,
            Issuer = TestIssuer,
            Audience = TestAudience,
            AccessTokenDurationMinutes = 15,
            RefreshTokenDurationDays = 7
        });

        _service = new JwtTokenService(options);
    }

    [Fact]
    public void GenerateTokens_ShouldReturnValidTokenResult()
    {
        // Act
        var result = _service.GenerateTokens(1, "admin@kouroukan.com", "Admin Principal", TestRoles, TestPermissions);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();
        result.AccessTokenExpiresAt.Should().BeAfter(DateTime.UtcNow);
        result.RefreshTokenExpiresAt.Should().BeAfter(DateTime.UtcNow);
        result.RefreshTokenExpiresAt.Should().BeAfter(result.AccessTokenExpiresAt);
    }

    [Fact]
    public void GenerateTokens_AccessToken_ShouldContainExpectedClaims()
    {
        // Act
        var result = _service.GenerateTokens(42, "ibrahima@kouroukan.com", "Ibrahima Doumbouya", TestRoles, TestPermissions);

        // Assert — valider et extraire les claims
        var principal = _service.ValidateAccessToken(result.AccessToken);
        principal.Should().NotBeNull();

        // Claim Sub
        principal!.FindFirst(JwtRegisteredClaimNames.Sub)?.Value.Should().Be("42");

        // Claim Email
        principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value.Should().Be("ibrahima@kouroukan.com");

        // Claim Name
        principal.FindFirst(JwtRegisteredClaimNames.Name)?.Value.Should().Be("Ibrahima Doumbouya");

        // Claim Jti (unique)
        principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value.Should().NotBeNullOrWhiteSpace();

        // Claims Role
        var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        roles.Should().Contain("directeur");
        roles.Should().Contain("enseignant");

        // Claims Permission
        var permissions = principal.FindAll("permission").Select(c => c.Value).ToList();
        permissions.Should().Contain("inscriptions:read");
        permissions.Should().Contain("inscriptions:write");
        permissions.Should().Contain("notes:read");
    }

    [Fact]
    public void ValidateAccessToken_ValidToken_ShouldReturnClaimsPrincipal()
    {
        // Arrange
        var result = _service.GenerateTokens(1, "test@test.com", "Test User", TestRoles, TestPermissions);

        // Act
        var principal = _service.ValidateAccessToken(result.AccessToken);

        // Assert
        principal.Should().NotBeNull();
        principal!.Identity.Should().NotBeNull();
        principal.Identity!.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void ValidateAccessToken_ExpiredToken_ShouldReturnNull()
    {
        // Arrange — creer un service avec une duree de -1 minute (deja expire)
        var options = Options.Create(new JwtOptions
        {
            Key = TestKey,
            Issuer = TestIssuer,
            Audience = TestAudience,
            AccessTokenDurationMinutes = -1,
            RefreshTokenDurationDays = 7
        });

        var expiredService = new JwtTokenService(options);
        var result = expiredService.GenerateTokens(1, "test@test.com", "Test", TestRoles, TestPermissions);

        // Act
        var principal = _service.ValidateAccessToken(result.AccessToken);

        // Assert
        principal.Should().BeNull("un token expire ne doit pas etre valide");
    }

    [Fact]
    public void ValidateAccessToken_TamperedToken_ShouldReturnNull()
    {
        // Arrange
        var result = _service.GenerateTokens(1, "test@test.com", "Test User", TestRoles, TestPermissions);
        var tamperedToken = result.AccessToken + "TAMPERED";

        // Act
        var principal = _service.ValidateAccessToken(tamperedToken);

        // Assert
        principal.Should().BeNull("un token altere ne doit pas etre valide");
    }

    [Fact]
    public void ValidateAccessToken_TokenWithWrongKey_ShouldReturnNull()
    {
        // Arrange — generer avec une cle differente
        var otherOptions = Options.Create(new JwtOptions
        {
            Key = "UneAutreCleCompletementDifferentePourHmacSha512QuiDoitFaireAuMoins64OctetsMinimum!!",
            Issuer = TestIssuer,
            Audience = TestAudience,
            AccessTokenDurationMinutes = 15,
            RefreshTokenDurationDays = 7
        });

        var otherService = new JwtTokenService(otherOptions);
        var result = otherService.GenerateTokens(1, "test@test.com", "Test", TestRoles, TestPermissions);

        // Act — valider avec la cle originale
        var principal = _service.ValidateAccessToken(result.AccessToken);

        // Assert
        principal.Should().BeNull("un token signe avec une cle differente ne doit pas etre valide");
    }

    [Fact]
    public void ValidateAccessToken_NonsenseString_ShouldReturnNull()
    {
        // Act
        var principal = _service.ValidateAccessToken("ceci.nest.pas.un.jwt");

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void GenerateTokens_MultipleCalls_ShouldProduceDifferentJti()
    {
        // Act
        var result1 = _service.GenerateTokens(1, "test@test.com", "Test", TestRoles, TestPermissions);
        var result2 = _service.GenerateTokens(1, "test@test.com", "Test", TestRoles, TestPermissions);

        // Assert
        var principal1 = _service.ValidateAccessToken(result1.AccessToken);
        var principal2 = _service.ValidateAccessToken(result2.AccessToken);

        var jti1 = principal1!.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var jti2 = principal2!.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        jti1.Should().NotBe(jti2, "chaque token doit avoir un identifiant unique");
    }

    [Fact]
    public void GenerateTokens_MultipleCalls_ShouldProduceDifferentRefreshTokens()
    {
        // Act
        var result1 = _service.GenerateTokens(1, "test@test.com", "Test", TestRoles, TestPermissions);
        var result2 = _service.GenerateTokens(1, "test@test.com", "Test", TestRoles, TestPermissions);

        // Assert
        result1.RefreshToken.Should().NotBe(result2.RefreshToken, "chaque refresh token doit etre unique");
    }

    [Fact]
    public void Constructor_KeyTooShort_ShouldThrowArgumentException()
    {
        // Arrange
        var options = Options.Create(new JwtOptions
        {
            Key = "CleTropCourte",
            Issuer = TestIssuer,
            Audience = TestAudience
        });

        // Act
        var act = () => new JwtTokenService(options);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*64 octets*");
    }
}
