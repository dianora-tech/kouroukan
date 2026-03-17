using Microsoft.Extensions.Logging;

namespace GnSecurity.Jwt;

/// <summary>
/// Service de gestion des refresh tokens avec rotation et detection de reutilisation.
/// <para>
/// Rotation : a chaque rafraichissement, l'ancien refresh token est revoque et remplace par un nouveau.
/// Detection de reutilisation : si un token deja revoque est presente, tous les tokens de l'utilisateur
/// sont revoques immediatement (indicateur de vol de token).
/// </para>
/// </summary>
public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenStore _store;
    private readonly IUserClaimsProvider _claimsProvider;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<RefreshTokenService> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="RefreshTokenService"/>.
    /// </summary>
    /// <param name="store">Store de persistance des refresh tokens.</param>
    /// <param name="claimsProvider">Fournisseur de claims utilisateur pour regenerer les tokens.</param>
    /// <param name="jwtTokenService">Service JWT pour generer les nouveaux tokens.</param>
    /// <param name="logger">Logger pour les evenements de securite.</param>
    public RefreshTokenService(
        IRefreshTokenStore store,
        IUserClaimsProvider claimsProvider,
        IJwtTokenService jwtTokenService,
        ILogger<RefreshTokenService> logger)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _claimsProvider = claimsProvider ?? throw new ArgumentNullException(nameof(claimsProvider));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<TokenResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken, nameof(refreshToken));

        var entry = await _store.GetByTokenAsync(refreshToken, cancellationToken).ConfigureAwait(false);

        // Token introuvable
        if (entry is null)
        {
            _logger.LogWarning("Tentative de rafraichissement avec un token introuvable.");
            throw new InvalidOperationException("Refresh token invalide.");
        }

        // Detection de reutilisation : token deja revoque => vol potentiel
        if (entry.IsRevoked)
        {
            _logger.LogWarning(
                "Detection de reutilisation de refresh token pour l'utilisateur {UserId}. Revocation de tous les tokens.",
                entry.UserId);

            await _store.RevokeAllForUserAsync(entry.UserId, cancellationToken).ConfigureAwait(false);
            throw new InvalidOperationException("Refresh token deja utilise. Tous les tokens ont ete revoques par securite.");
        }

        // Token expire
        if (entry.ExpiresAt <= DateTime.UtcNow)
        {
            _logger.LogInformation("Refresh token expire pour l'utilisateur {UserId}.", entry.UserId);
            throw new InvalidOperationException("Refresh token expire.");
        }

        // Recuperer les claims actuels de l'utilisateur
        var userClaims = await _claimsProvider.GetUserClaimsAsync(entry.UserId, cancellationToken).ConfigureAwait(false);
        if (userClaims is null)
        {
            _logger.LogWarning("Utilisateur {UserId} introuvable ou inactif lors du rafraichissement.", entry.UserId);
            await _store.RevokeAsync(refreshToken, null, cancellationToken).ConfigureAwait(false);
            throw new InvalidOperationException("Utilisateur introuvable ou inactif.");
        }

        // Generer de nouveaux tokens
        var newTokens = _jwtTokenService.GenerateTokens(
            userClaims.UserId,
            userClaims.Email,
            userClaims.FullName,
            userClaims.Roles,
            userClaims.Permissions);

        // Rotation : revoquer l'ancien et stocker le nouveau
        await _store.RevokeAsync(refreshToken, newTokens.RefreshToken, cancellationToken).ConfigureAwait(false);
        await _store.StoreAsync(
            entry.UserId,
            newTokens.RefreshToken,
            newTokens.RefreshTokenExpiresAt,
            cancellationToken).ConfigureAwait(false);

        _logger.LogInformation(
            "Refresh token effectue avec succes pour l'utilisateur {UserId}.",
            entry.UserId);

        return newTokens;
    }

    /// <inheritdoc />
    public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken, nameof(refreshToken));

        var entry = await _store.GetByTokenAsync(refreshToken, cancellationToken).ConfigureAwait(false);
        if (entry is null)
        {
            _logger.LogWarning("Tentative de revocation d'un token introuvable.");
            return;
        }

        if (entry.IsRevoked)
        {
            _logger.LogInformation("Token deja revoque pour l'utilisateur {UserId}.", entry.UserId);
            return;
        }

        await _store.RevokeAsync(refreshToken, null, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation(
            "Refresh token revoque manuellement pour l'utilisateur {UserId}.",
            entry.UserId);
    }
}
