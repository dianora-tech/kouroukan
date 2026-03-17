using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Kouroukan.Api.Gateway.HealthChecks;

/// <summary>
/// Health check pour PostgreSQL via Npgsql.
/// Execute un SELECT 1 avec un timeout de 5 secondes.
/// </summary>
public sealed class PostgreSqlHealthCheck : IHealthCheck
{
    private readonly string _connectionString;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="PostgreSqlHealthCheck"/>.
    /// </summary>
    public PostgreSqlHealthCheck(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' non configuree.");
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            command.CommandTimeout = 5;

            await command.ExecuteScalarAsync(cancellationToken);

            return HealthCheckResult.Healthy("PostgreSQL est accessible.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL est inaccessible.", ex);
        }
    }
}
