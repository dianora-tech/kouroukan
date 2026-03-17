using System.Security.Claims;

namespace GnSecurity.Jwt;

/// <summary>
/// Service de generation et validation des tokens JWT.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Genere un access token JWT et un refresh token opaque.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur (claim sub).</param>
    /// <param name="email">Email de l'utilisateur (claim email).</param>
    /// <param name="fullName">Nom complet de l'utilisateur (claim name).</param>
    /// <param name="roles">Liste des roles de l'utilisateur (claims role).</param>
    /// <param name="permissions">Liste des permissions de l'utilisateur (claims permission).</param>
    /// <returns>Un <see cref="TokenResult"/> contenant les tokens et leurs dates d'expiration.</returns>
    TokenResult GenerateTokens(
        int userId,
        string email,
        string fullName,
        IEnumerable<string> roles,
        IEnumerable<string> permissions);

    /// <summary>
    /// Valide un access token JWT et retourne le <see cref="ClaimsPrincipal"/> associe.
    /// </summary>
    /// <param name="token">Access token JWT a valider.</param>
    /// <returns>Le <see cref="ClaimsPrincipal"/> si le token est valide, <c>null</c> sinon.</returns>
    ClaimsPrincipal? ValidateAccessToken(string token);
}
