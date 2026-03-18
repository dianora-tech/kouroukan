using Kouroukan.Api.Gateway.Models;

namespace Kouroukan.Api.Gateway.Auth;

/// <summary>
/// Service de haut niveau pour la gestion des tokens d'authentification.
/// Orchestre IJwtTokenService, IRefreshTokenStore et les queries utilisateur.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Authentifie un utilisateur et genere les tokens.
    /// </summary>
    /// <param name="email">Email de l'utilisateur.</param>
    /// <param name="password">Mot de passe en clair.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Les tokens d'authentification.</returns>
    Task<AuthTokensDto> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupere le profil de l'utilisateur par son identifiant.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Le profil utilisateur.</returns>
    Task<UserProfileDto?> GetUserProfileAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupere la version active des CGU.
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>La version active des CGU.</returns>
    Task<CguVersionDto?> GetActiveCguAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Enregistre l'acceptation des CGU et retourne un nouveau token.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="cguVersion">Version des CGU acceptees.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Nouveaux tokens avec le claim cguVersion mis a jour.</returns>
    Task<AuthTokensDto> AcceptCguAsync(int userId, string cguVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inscrit un nouvel etablissement : cree l'utilisateur directeur, l'etablissement,
    /// assigne le role et optionnellement enregistre la localisation.
    /// </summary>
    /// <param name="request">Donnees d'inscription.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Les tokens d'authentification du directeur inscrit.</returns>
    Task<AuthTokensDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}
