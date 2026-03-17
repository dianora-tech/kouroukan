using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using Dapper;
using GnDapper.Attributes;
using GnDapper.Connection;
using GnDapper.Entities;
using GnDapper.Exceptions;
using GnDapper.Guards;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Specifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GnDapper.Repositories;

/// <summary>
/// Implementation generique du repository utilisant Dapper pour PostgreSQL.
/// Genere dynamiquement les requetes SQL a partir des metadonnees de l'entite.
/// </summary>
/// <typeparam name="T">Type de l'entite implementant <see cref="IEntity"/>.</typeparam>
public class Repository<T> : IRepository<T> where T : class, IEntity
{
    private static readonly ConcurrentDictionary<Type, EntityMetadata> MetadataCache = new();

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<Repository<T>> _logger;
    private readonly GnDapperOptions _options;

    /// <summary>
    /// Metadonnees de l'entite (nom de table, colonnes).
    /// </summary>
    protected EntityMetadata Metadata { get; }

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="Repository{T}"/>.
    /// </summary>
    /// <param name="connectionFactory">Fabrique de connexions a la base de donnees.</param>
    /// <param name="logger">Logger pour la journalisation.</param>
    /// <param name="options">Options de configuration GnDapper.</param>
    public Repository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<T>> logger,
        IOptions<GnDapperOptions> options)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        _options = options.Value;

        Metadata = MetadataCache.GetOrAdd(typeof(T), _ => BuildMetadata());
    }

    /// <summary>
    /// Fabrique de connexions accessible aux classes derivees.
    /// </summary>
    protected IDbConnectionFactory ConnectionFactory => _connectionFactory;

    /// <summary>
    /// Logger accessible aux classes derivees.
    /// </summary>
    protected ILogger<Repository<T>> Logger => _logger;

    /// <summary>
    /// Options de configuration accessibles aux classes derivees.
    /// </summary>
    protected GnDapperOptions Options => _options;

    // ========================================================================
    // CRUD
    // ========================================================================

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT * FROM {Metadata.TableName} WHERE id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { Id = id },
            commandTimeout: _options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<T>(command)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT * FROM {Metadata.TableName}";

        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql,
            commandTimeout: _options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

        var result = await connection.QueryAsync<T>(command)
            .ConfigureAwait(false);

        return result.AsList().AsReadOnly();
    }

    /// <inheritdoc />
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        var columns = Metadata.InsertColumns;
        var columnNames = string.Join(", ", columns.Select(c => c.ColumnName));
        var paramNames = string.Join(", ", columns.Select(c => $"@{c.PropertyName}"));
        var sql = $"INSERT INTO {Metadata.TableName} ({columnNames}) VALUES ({paramNames}) RETURNING *";

        _logger.LogDebug("Insertion dans {Table} : {Sql}", Metadata.TableName, sql);

        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, entity,
            commandTimeout: _options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

        var result = await connection.QuerySingleAsync<T>(command)
            .ConfigureAwait(false);

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        var columns = Metadata.UpdateColumns;
        var setClauses = string.Join(", ", columns.Select(c => $"{c.ColumnName} = @{c.PropertyName}"));
        var sql = $"UPDATE {Metadata.TableName} SET {setClauses} WHERE id = @Id";

        _logger.LogDebug("Mise a jour dans {Table} : {Sql}", Metadata.TableName, sql);

        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, entity,
            commandTimeout: _options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

        var affected = await connection.ExecuteAsync(command)
            .ConfigureAwait(false);

        return affected > 0;
    }

    /// <inheritdoc />
    public virtual async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"DELETE FROM {Metadata.TableName} WHERE id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { Id = id },
            commandTimeout: _options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

        var affected = await connection.ExecuteAsync(command)
            .ConfigureAwait(false);

        return affected > 0;
    }

    /// <inheritdoc />
    public virtual async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT EXISTS(SELECT 1 FROM {Metadata.TableName} WHERE id = @Id)";

        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { Id = id },
            commandTimeout: _options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<bool>(command)
            .ConfigureAwait(false);
    }

    // ========================================================================
    // Requetes brutes
    // ========================================================================

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<T>> GetWithQueryAsync(
        string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql, nameof(sql));

        if (_options.EnableSqlInjectionGuard)
        {
            SqlInjectionGuard.Validate(sql);
        }

        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, param,
            commandTimeout: _options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

        var result = await connection.QueryAsync<T>(command)
            .ConfigureAwait(false);

        return result.AsList().AsReadOnly();
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetSingleWithQueryAsync(
        string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql, nameof(sql));

        if (_options.EnableSqlInjectionGuard)
        {
            SqlInjectionGuard.Validate(sql);
        }

        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, param,
            commandTimeout: _options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<T>(command)
            .ConfigureAwait(false);
    }

    // ========================================================================
    // Specification
    // ========================================================================

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<T>> FindAsync(
        ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(spec, nameof(spec));

        var sql = BuildSelectSql(spec);

        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, spec.Parameters,
            commandTimeout: _options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

        var result = await connection.QueryAsync<T>(command)
            .ConfigureAwait(false);

        return result.AsList().AsReadOnly();
    }

    /// <inheritdoc />
    public virtual async Task<PagedResult<T>> FindPagedAsync(
        ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(spec, nameof(spec));

        var selectSql = BuildSelectSql(spec);
        var countSql = BuildCountSql(spec);

        using var connection = _connectionFactory.CreateConnection();

        var countCommand = new CommandDefinition(countSql, spec.Parameters,
            commandTimeout: _options.CommandTimeoutSeconds, cancellationToken: cancellationToken);
        var totalCount = await connection.ExecuteScalarAsync<int>(countCommand)
            .ConfigureAwait(false);

        var selectCommand = new CommandDefinition(selectSql, spec.Parameters,
            commandTimeout: _options.CommandTimeoutSeconds, cancellationToken: cancellationToken);
        var items = await connection.QueryAsync<T>(selectCommand)
            .ConfigureAwait(false);

        var page = spec.Skip.HasValue && spec.Take.HasValue && spec.Take.Value > 0
            ? (spec.Skip.Value / spec.Take.Value) + 1
            : 1;
        var pageSize = spec.Take ?? totalCount;

        return new PagedResult<T>(items.AsList().AsReadOnly(), totalCount, page, pageSize);
    }

    /// <inheritdoc />
    public virtual async Task<int> CountAsync(
        ISpecification<T>? spec = null, CancellationToken cancellationToken = default)
    {
        var countSql = spec is not null
            ? BuildCountSql(spec)
            : $"SELECT COUNT(*) FROM {Metadata.TableName}";

        using var connection = _connectionFactory.CreateConnection();
        var command = new CommandDefinition(countSql, spec?.Parameters,
            commandTimeout: _options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<int>(command)
            .ConfigureAwait(false);
    }

    // ========================================================================
    // Operations en masse (Bulk)
    // ========================================================================

    /// <inheritdoc />
    public virtual async Task<int> BulkInsertAsync(
        IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));

        var entityList = entities.ToList();
        if (entityList.Count == 0) return 0;

        var columns = Metadata.InsertColumns;
        var columnNames = string.Join(", ", columns.Select(c => c.ColumnName));
        var paramNames = string.Join(", ", columns.Select(c => $"@{c.PropertyName}"));
        var sql = $"INSERT INTO {Metadata.TableName} ({columnNames}) VALUES ({paramNames})";

        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        using var transaction = await connection.BeginTransactionAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var totalInserted = 0;

            foreach (var entity in entityList)
            {
                var command = new CommandDefinition(sql, entity,
                    transaction: transaction,
                    commandTimeout: _options.CommandTimeoutSeconds,
                    cancellationToken: cancellationToken);

                totalInserted += await connection.ExecuteAsync(command)
                    .ConfigureAwait(false);
            }

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation(
                "Insertion en masse reussie : {Count} entite(s) inseree(s) dans {Table}.",
                totalInserted, Metadata.TableName);

            return totalInserted;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogError(ex,
                "Erreur lors de l'insertion en masse dans {Table}. Transaction annulee.",
                Metadata.TableName);

            throw new DataAccessException(
                $"Erreur lors de l'insertion en masse dans {Metadata.TableName}.", ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<int> BulkUpdateAsync(
        IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));

        var entityList = entities.ToList();
        if (entityList.Count == 0) return 0;

        var columns = Metadata.UpdateColumns;
        var setClauses = string.Join(", ", columns.Select(c => $"{c.ColumnName} = @{c.PropertyName}"));
        var sql = $"UPDATE {Metadata.TableName} SET {setClauses} WHERE id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        using var transaction = await connection.BeginTransactionAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var totalUpdated = 0;

            foreach (var entity in entityList)
            {
                var command = new CommandDefinition(sql, entity,
                    transaction: transaction,
                    commandTimeout: _options.CommandTimeoutSeconds,
                    cancellationToken: cancellationToken);

                totalUpdated += await connection.ExecuteAsync(command)
                    .ConfigureAwait(false);
            }

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation(
                "Mise a jour en masse reussie : {Count} entite(s) mise(s) a jour dans {Table}.",
                totalUpdated, Metadata.TableName);

            return totalUpdated;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogError(ex,
                "Erreur lors de la mise a jour en masse dans {Table}. Transaction annulee.",
                Metadata.TableName);

            throw new DataAccessException(
                $"Erreur lors de la mise a jour en masse dans {Metadata.TableName}.", ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task<int> BulkDeleteAsync(
        IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids, nameof(ids));

        var idList = ids.ToList();
        if (idList.Count == 0) return 0;

        var sql = $"DELETE FROM {Metadata.TableName} WHERE id = ANY(@Ids)";

        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        using var transaction = await connection.BeginTransactionAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var command = new CommandDefinition(sql, new { Ids = idList.ToArray() },
                transaction: transaction,
                commandTimeout: _options.CommandTimeoutSeconds,
                cancellationToken: cancellationToken);

            var totalDeleted = await connection.ExecuteAsync(command)
                .ConfigureAwait(false);

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation(
                "Suppression en masse reussie : {Count} entite(s) supprimee(s) dans {Table}.",
                totalDeleted, Metadata.TableName);

            return totalDeleted;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogError(ex,
                "Erreur lors de la suppression en masse dans {Table}. Transaction annulee.",
                Metadata.TableName);

            throw new DataAccessException(
                $"Erreur lors de la suppression en masse dans {Metadata.TableName}.", ex);
        }
    }

    // ========================================================================
    // SQL Builder helpers
    // ========================================================================

    /// <summary>
    /// Construit la requete SELECT a partir d'une specification.
    /// </summary>
    protected string BuildSelectSql(ISpecification<T> spec)
    {
        var sb = new StringBuilder($"SELECT * FROM {Metadata.TableName}");

        if (!string.IsNullOrWhiteSpace(spec.WhereClause))
        {
            sb.Append($" WHERE {spec.WhereClause}");
        }

        if (!string.IsNullOrWhiteSpace(spec.OrderByClause))
        {
            sb.Append($" ORDER BY {spec.OrderByClause}");
        }

        if (spec.Take.HasValue)
        {
            sb.Append($" LIMIT {spec.Take.Value}");
        }

        if (spec.Skip.HasValue)
        {
            sb.Append($" OFFSET {spec.Skip.Value}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Construit la requete COUNT a partir d'une specification.
    /// </summary>
    protected string BuildCountSql(ISpecification<T> spec)
    {
        var sb = new StringBuilder($"SELECT COUNT(*) FROM {Metadata.TableName}");

        if (!string.IsNullOrWhiteSpace(spec.WhereClause))
        {
            sb.Append($" WHERE {spec.WhereClause}");
        }

        return sb.ToString();
    }

    // ========================================================================
    // Metadata
    // ========================================================================

    private static EntityMetadata BuildMetadata()
    {
        var type = typeof(T);
        var tableAttr = type.GetCustomAttribute<TableAttribute>();
        var tableName = tableAttr?.Name ?? ConvertToSnakeCase(type.Name);

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite)
            .ToArray();

        var allColumns = properties
            .Select(p => new ColumnMapping(p.Name, ConvertToSnakeCase(p.Name)))
            .ToArray();

        // Exclure Id pour INSERT (GENERATED ALWAYS AS IDENTITY)
        var insertColumns = allColumns
            .Where(c => !string.Equals(c.PropertyName, "Id", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        // Exclure Id pour UPDATE
        var updateColumns = allColumns
            .Where(c => !string.Equals(c.PropertyName, "Id", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return new EntityMetadata(tableName, allColumns, insertColumns, updateColumns);
    }

    private static string ConvertToSnakeCase(string name)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < name.Length; i++)
        {
            var c = name[i];

            if (char.IsUpper(c))
            {
                if (i > 0)
                {
                    sb.Append('_');
                }

                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Metadonnees d'une entite (nom de table et colonnes).
    /// </summary>
    /// <param name="TableName">Nom complet de la table PostgreSQL.</param>
    /// <param name="AllColumns">Toutes les colonnes de l'entite.</param>
    /// <param name="InsertColumns">Colonnes pour les operations INSERT (sans Id).</param>
    /// <param name="UpdateColumns">Colonnes pour les operations UPDATE (sans Id).</param>
    protected record EntityMetadata(
        string TableName,
        ColumnMapping[] AllColumns,
        ColumnMapping[] InsertColumns,
        ColumnMapping[] UpdateColumns);

    /// <summary>
    /// Mapping entre le nom de la propriete C# et le nom de la colonne PostgreSQL.
    /// </summary>
    /// <param name="PropertyName">Nom de la propriete C# (PascalCase).</param>
    /// <param name="ColumnName">Nom de la colonne PostgreSQL (snake_case).</param>
    protected record ColumnMapping(string PropertyName, string ColumnName);
}
