using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kouroukan.Api.Gateway.HealthChecks;

/// <summary>
/// Health check generique pour les microservices downstream.
/// Verifie l'accessibilite de chaque service via son endpoint /health.
/// </summary>
public sealed class DownstreamServiceHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _serviceName;
    private readonly string _serviceUrl;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="DownstreamServiceHealthCheck"/>.
    /// </summary>
    public DownstreamServiceHealthCheck(
        IHttpClientFactory httpClientFactory,
        string serviceName,
        string serviceUrl)
    {
        _httpClientFactory = httpClientFactory;
        _serviceName = serviceName;
        _serviceUrl = serviceUrl;
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

            var response = await client.GetAsync($"{_serviceUrl}/health", cancellationToken);

            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy($"{_serviceName} est accessible.")
                : HealthCheckResult.Degraded($"{_serviceName} a repondu avec le code {(int)response.StatusCode}.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"{_serviceName} est inaccessible.", ex);
        }
    }
}

/// <summary>
/// Factory pour creer des health checks parametres pour chaque service downstream.
/// </summary>
public static class DownstreamServiceHealthCheckFactory
{
    /// <summary>
    /// Enregistre les health checks pour tous les microservices downstream.
    /// </summary>
    public static IHealthChecksBuilder AddDownstreamServices(this IHealthChecksBuilder builder, IConfiguration configuration)
    {
        var services = new Dictionary<string, string>
        {
            ["inscriptions"] = "http://inscriptions-service:5001",
            ["pedagogie"] = "http://pedagogie-service:5002",
            ["evaluations"] = "http://evaluations-service:5003",
            ["presences"] = "http://presences-service:5004",
            ["finances"] = "http://finances-service:5005",
            ["personnel"] = "http://personnel-service:5006",
            ["communication"] = "http://communication-service:5007",
            ["bde"] = "http://bde-service:5008",
            ["documents"] = "http://documents-service:5009",
            ["services-premium"] = "http://services-premium-service:5010",
            ["support"] = "http://support-service:5011",
            ["cache"] = "http://cache-service:5100"
        };

        foreach (var (name, url) in services)
        {
            var serviceUrl = configuration[$"HealthChecks:Services:{name}"] ?? url;
            builder.Add(new HealthCheckRegistration(
                $"downstream-{name}",
                sp => new DownstreamServiceHealthCheck(
                    sp.GetRequiredService<IHttpClientFactory>(),
                    name,
                    serviceUrl),
                failureStatus: HealthStatus.Degraded,
                tags: ["downstream"]));
        }

        return builder;
    }
}
