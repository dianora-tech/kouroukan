using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kouroukan.Api.Gateway.Services;

/// <summary>
/// Implementation du service de validation Cloudflare Turnstile.
/// Appelle l'endpoint siteverify de Cloudflare pour valider le token client.
/// </summary>
/// <remarks>
/// Documentation : https://developers.cloudflare.com/turnstile/get-started/server-side-validation/
/// </remarks>
public sealed class TurnstileService : ITurnstileService
{
    private readonly HttpClient _httpClient;
    private readonly string _secretKey;
    private readonly ILogger<TurnstileService> _logger;

    private const string VerifyUrl = "https://challenges.cloudflare.com/turnstile/v0/siteverify";

    public TurnstileService(HttpClient httpClient, IConfiguration configuration, ILogger<TurnstileService> logger)
    {
        _httpClient = httpClient;
        _secretKey = configuration["Turnstile:SecretKey"] ?? string.Empty;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> ValidateAsync(string token, string? remoteIp = null, CancellationToken cancellationToken = default)
    {
        // Si pas de secret key configuree, on skip la validation (dev local)
        if (string.IsNullOrEmpty(_secretKey))
        {
            _logger.LogWarning("Turnstile secret key non configuree — validation ignoree");
            return true;
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("Turnstile token vide ou null");
            return false;
        }

        try
        {
            var payload = new Dictionary<string, string>
            {
                ["secret"] = _secretKey,
                ["response"] = token
            };

            if (!string.IsNullOrEmpty(remoteIp))
            {
                payload["remoteip"] = remoteIp;
            }

            var content = new FormUrlEncodedContent(payload);
            var response = await _httpClient.PostAsync(VerifyUrl, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<TurnstileResponse>(json);

            if (result is null)
            {
                _logger.LogWarning("Turnstile : reponse de validation nulle");
                return false;
            }

            if (!result.Success)
            {
                _logger.LogWarning("Turnstile validation echouee : {Errors}",
                    string.Join(", ", result.ErrorCodes ?? Array.Empty<string>()));
            }

            return result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation Turnstile");
            // En cas d'erreur reseau avec Cloudflare, on laisse passer
            // pour ne pas bloquer les utilisateurs legitimes
            return true;
        }
    }

    private sealed class TurnstileResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error-codes")]
        public string[]? ErrorCodes { get; set; }

        [JsonPropertyName("challenge_ts")]
        public string? ChallengeTs { get; set; }

        [JsonPropertyName("hostname")]
        public string? Hostname { get; set; }
    }
}
