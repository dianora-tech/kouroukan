using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GnSecurity.Jwt;

/// <summary>
/// Implementation du service JWT utilisant HMAC-SHA512.
/// Claims : Sub (user id), Email, Name, Jti (unique id), Role[] et Permission[].
/// </summary>
public sealed class JwtTokenService : IJwtTokenService
{
    /// <summary>Nom du claim personnalise pour les permissions.</summary>
    private const string PermissionClaimType = "permission";

    private readonly JwtOptions _options;
    private readonly SigningCredentials _signingCredentials;
    private readonly TokenValidationParameters _validationParameters;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="JwtTokenService"/>.
    /// </summary>
    /// <param name="options">Options JWT configurees via IOptions.</param>
    /// <exception cref="ArgumentException">Si la cle est trop courte pour HMAC-SHA512.</exception>
    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;

        var keyBytes = Encoding.UTF8.GetBytes(_options.Key);
        if (keyBytes.Length < 64)
            throw new ArgumentException(
                "La cle JWT doit comporter au minimum 64 octets (512 bits) pour HMAC-SHA512.",
                nameof(options));

        var securityKey = new SymmetricSecurityKey(keyBytes);
        _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    }

    /// <inheritdoc />
    public TokenResult GenerateTokens(
        int userId,
        string email,
        string fullName,
        IEnumerable<string> roles,
        IEnumerable<string> permissions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName, nameof(fullName));
        ArgumentNullException.ThrowIfNull(roles, nameof(roles));
        ArgumentNullException.ThrowIfNull(permissions, nameof(permissions));

        var now = DateTime.UtcNow;
        var accessTokenExpires = now.AddMinutes(_options.AccessTokenDurationMinutes);
        var refreshTokenExpires = now.AddDays(_options.RefreshTokenDurationDays);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString(), ClaimValueTypes.Integer32),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Name, fullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var permission in permissions)
        {
            claims.Add(new Claim(PermissionClaimType, permission));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = accessTokenExpires,
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            SigningCredentials = _signingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);

        var refreshToken = GenerateRefreshToken();

        return new TokenResult(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            AccessTokenExpiresAt: accessTokenExpires,
            RefreshTokenExpiresAt: refreshTokenExpires
        );
    }

    /// <inheritdoc />
    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token, nameof(token));

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, _validationParameters, out _);
            return principal;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    /// <summary>
    /// Genere un refresh token opaque cryptographiquement securise (Base64, 64 octets).
    /// </summary>
    /// <returns>Refresh token sous forme de chaine Base64.</returns>
    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
