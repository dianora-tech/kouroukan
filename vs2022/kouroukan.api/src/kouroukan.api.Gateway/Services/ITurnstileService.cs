namespace Kouroukan.Api.Gateway.Services;

/// <summary>
/// Service de validation Cloudflare Turnstile.
/// Verifie que le token genere cote client est valide (protection anti-bot).
/// </summary>
public interface ITurnstileService
{
    /// <summary>
    /// Valide un token Turnstile aupres de l'API Cloudflare.
    /// </summary>
    /// <param name="token">Token genere par le widget Turnstile cote client.</param>
    /// <param name="remoteIp">Adresse IP du client (optionnel).</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>true si le token est valide, false sinon.</returns>
    Task<bool> ValidateAsync(string token, string? remoteIp = null, CancellationToken cancellationToken = default);
}
