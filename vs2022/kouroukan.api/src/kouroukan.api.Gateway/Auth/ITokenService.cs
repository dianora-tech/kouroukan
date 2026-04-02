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
    /// Change le mot de passe de l'utilisateur et desactive le flag must_change_password.
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur.</param>
    /// <param name="currentPassword">Mot de passe actuel.</param>
    /// <param name="newPassword">Nouveau mot de passe.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    Task ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inscrit un nouvel etablissement : cree l'utilisateur directeur, l'etablissement,
    /// assigne le role et optionnellement enregistre la localisation.
    /// </summary>
    /// <param name="request">Donnees d'inscription.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Les tokens d'authentification du directeur inscrit.</returns>
    Task<AuthTokensDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met a jour les preferences utilisateur (langue, theme).
    /// </summary>
    Task UpdatePreferencesAsync(int userId, string locale, string theme, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupere le statut d'onboarding de l'etablissement de l'utilisateur.
    /// </summary>
    Task<OnboardingStatusDto?> GetOnboardingStatusAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met a jour l'etape d'onboarding de l'etablissement.
    /// </summary>
    Task UpdateOnboardingStepAsync(int userId, int step, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marque l'onboarding comme termine (skip ou completion).
    /// </summary>
    Task CompleteOnboardingAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met a jour les informations de l'etablissement.
    /// </summary>
    Task UpdateCompanyAsync(int userId, UpdateCompanyRequest request, CancellationToken cancellationToken = default);
}
