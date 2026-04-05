using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kouroukan.Api.Gateway.Services;

/// <summary>
/// Service d'integration avec l'API NimbaSMS (fournisseur SMS guineen).
/// API Base: https://api.nimbasms.com/v1
/// Auth: Basic (SERVICE_ID:SECRET_TOKEN)
/// </summary>
public class NimbaSmsService
{
    private const string BaseUrl = "https://api.nimbasms.com/v1";
    private readonly HttpClient _httpClient;
    private readonly ILogger<NimbaSmsService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public NimbaSmsService(HttpClient httpClient, ILogger<NimbaSmsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    private void SetAuth(string serviceId, string secretToken)
    {
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{serviceId}:{secretToken}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
    }

    /// <summary>
    /// Envoie un SMS via NimbaSMS.
    /// </summary>
    public async Task<NimbaSendResult> SendSmsAsync(string serviceId, string secretToken, string senderName, string to, string message)
    {
        SetAuth(serviceId, secretToken);

        var payload = new
        {
            to = new[] { to.Replace("+", "").Replace(" ", "") },
            sender_name = senderName,
            message
        };

        var content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync($"{BaseUrl}/messages", content);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("SMS envoye a {To} via NimbaSMS", to);
                return new NimbaSendResult { Success = true, Response = body };
            }

            _logger.LogError("Erreur NimbaSMS {StatusCode}: {Body}", response.StatusCode, body);
            return new NimbaSendResult { Success = false, Error = body };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur de connexion a NimbaSMS");
            return new NimbaSendResult { Success = false, Error = ex.Message };
        }
    }

    /// <summary>
    /// Recupere le solde du compte NimbaSMS.
    /// </summary>
    public async Task<int> GetBalanceAsync(string serviceId, string secretToken)
    {
        SetAuth(serviceId, secretToken);

        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/accounts");
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("balance", out var balanceProp))
                {
                    return balanceProp.GetInt32();
                }
            }

            _logger.LogWarning("Impossible de recuperer le solde NimbaSMS: {Body}", body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la recuperation du solde NimbaSMS");
        }

        return 0;
    }
}

public class NimbaSendResult
{
    public bool Success { get; set; }
    public string? Response { get; set; }
    public string? Error { get; set; }
}
