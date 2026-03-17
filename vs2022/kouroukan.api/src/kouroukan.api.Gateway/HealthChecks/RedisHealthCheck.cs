using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Kouroukan.Api.Gateway.HealthChecks;

/// <summary>
/// Health check pour Redis via StackExchange.Redis.
/// Execute un PING.
/// </summary>
public sealed class RedisHealthCheck : IHealthCheck
{
    private readonly string _connectionString;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="RedisHealthCheck"/>.
    /// </summary>
    public RedisHealthCheck(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("ConnectionString 'Redis' non configuree.");
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = await ConnectionMultiplexer.ConnectAsync(_connectionString);
            var db = connection.GetDatabase();
            var pingResult = await db.PingAsync();

            return HealthCheckResult.Healthy($"Redis est accessible. Ping: {pingResult.TotalMilliseconds}ms");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis est inaccessible.", ex);
        }
    }
}
