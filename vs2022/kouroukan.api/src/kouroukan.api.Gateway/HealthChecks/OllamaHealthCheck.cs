using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kouroukan.Api.Gateway.HealthChecks;

/// <summary>
/// Health check pour Ollama (IA generative).
/// Verifie que le service est accessible et que le modele "mistral" est charge.
/// </summary>
public sealed class OllamaHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="OllamaHealthCheck"/>.
    /// </summary>
    public OllamaHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = configuration["Ollama:BaseUrl"] ?? "http://ollama:11434";
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            var response = await client.GetAsync($"{_baseUrl}/api/tags", cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var doc = JsonDocument.Parse(content);

            if (doc.RootElement.TryGetProperty("models", out var models))
            {
                foreach (var model in models.EnumerateArray())
                {
                    var name = model.GetProperty("name").GetString() ?? string.Empty;
                    if (name.Contains("mistral", StringComparison.OrdinalIgnoreCase))
                    {
                        return HealthCheckResult.Healthy("Ollama est accessible, modele mistral charge.");
                    }
                }
            }

            return HealthCheckResult.Degraded("Ollama est accessible mais le modele mistral n'est pas charge.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Ollama est inaccessible.", ex);
        }
    }
}
