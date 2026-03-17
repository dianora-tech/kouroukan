using System.Data.Common;
using GnDapper.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace GnDapper.Connection;

/// <summary>
/// Implementation de <see cref="IDbConnectionFactory"/> utilisant Npgsql pour PostgreSQL.
/// Enregistree en Singleton — le pool de connexions est gere internement par Npgsql.
/// </summary>
public sealed class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="NpgsqlConnectionFactory"/>.
    /// </summary>
    /// <param name="options">Options de configuration contenant la chaine de connexion.</param>
    /// <exception cref="ArgumentNullException">Si <paramref name="options"/> est null.</exception>
    /// <exception cref="ArgumentException">Si la chaine de connexion est vide.</exception>
    public NpgsqlConnectionFactory(IOptions<GnDapperOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.ConnectionString, nameof(options.Value.ConnectionString));

        _connectionString = options.Value.ConnectionString;
    }

    /// <inheritdoc />
    public DbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
