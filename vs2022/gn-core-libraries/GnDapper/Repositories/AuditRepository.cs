using System.Text;
using Dapper;
using GnDapper.Connection;
using GnDapper.Entities;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GnDapper.Repositories;

/// <summary>
/// Repository avec support automatique de l'audit et de la suppression logique.
/// Herite de <see cref="Repository{T}"/> et surcharge les operations CRUD pour :
/// <list type="bullet">
/// <item>Remplir automatiquement CreatedAt/CreatedBy lors de l'insertion.</item>
/// <item>Remplir automatiquement UpdatedAt/UpdatedBy lors de la mise a jour.</item>
/// <item>Effectuer une suppression logique (soft delete) au lieu de physique.</item>
/// <item>Filtrer automatiquement les entites supprimees dans les requetes.</item>
/// </list>
/// </summary>
/// <typeparam name="T">Type de l'entite implementant <see cref="IAuditableEntity"/> et <see cref="ISoftDeletable"/>.</typeparam>
public sealed class AuditRepository<T> : Repository<T>
    where T : class, IAuditableEntity, ISoftDeletable, new()
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="AuditRepository{T}"/>.
    /// </summary>
    /// <param name="connectionFactory">Fabrique de connexions a la base de donnees.</param>
    /// <param name="logger">Logger pour la journalisation.</param>
    /// <param name="options">Options de configuration GnDapper.</param>
    /// <param name="httpContextAccessor">Accesseur du contexte HTTP pour recuperer l'utilisateur courant.</param>
    public AuditRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<T>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
        : base(connectionFactory, logger, options)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <inheritdoc />
    public override async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT * FROM {Metadata.TableName} WHERE id = @Id AND is_deleted = FALSE";

        using var connection = ConnectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { Id = id },
            commandTimeout: Options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<T>(command)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT * FROM {Metadata.TableName} WHERE is_deleted = FALSE";

        using var connection = ConnectionFactory.CreateConnection();
        var command = new CommandDefinition(sql,
            commandTimeout: Options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

        var result = await connection.QueryAsync<T>(command)
            .ConfigureAwait(false);

        return result.AsList().AsReadOnly();
    }

    /// <inheritdoc />
    public override async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        var currentUser = GetCurrentUser();

        entity.CreatedAt = DateTime.UtcNow;
        entity.CreatedBy = currentUser;
        entity.IsDeleted = false;

        Logger.LogDebug(
            "Audit : creation dans {Table} par {User}.",
            Metadata.TableName, currentUser);

        return await base.AddAsync(entity, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        var currentUser = GetCurrentUser();

        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = currentUser;

        Logger.LogDebug(
            "Audit : mise a jour dans {Table} de l'entite {Id} par {User}.",
            Metadata.TableName, entity.Id, currentUser);

        return await base.UpdateAsync(entity, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var currentUser = GetCurrentUser();
        var now = DateTime.UtcNow;

        var sql = $"UPDATE {Metadata.TableName} SET is_deleted = TRUE, deleted_at = @DeletedAt, deleted_by = @DeletedBy WHERE id = @Id AND is_deleted = FALSE";

        Logger.LogDebug(
            "Audit : suppression logique dans {Table} de l'entite {Id} par {User}.",
            Metadata.TableName, id, currentUser);

        using var connection = ConnectionFactory.CreateConnection();
        var command = new CommandDefinition(sql,
            new { Id = id, DeletedAt = now, DeletedBy = currentUser },
            commandTimeout: Options.CommandTimeoutSeconds,
            cancellationToken: cancellationToken);

        var affected = await connection.ExecuteAsync(command)
            .ConfigureAwait(false);

        return affected > 0;
    }

    /// <inheritdoc />
    public override async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT EXISTS(SELECT 1 FROM {Metadata.TableName} WHERE id = @Id AND is_deleted = FALSE)";

        using var connection = ConnectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { Id = id },
            commandTimeout: Options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<bool>(command)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<IReadOnlyList<T>> FindAsync(
        ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(spec, nameof(spec));

        var sql = BuildSelectSqlWithSoftDelete(spec);

        using var connection = ConnectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, spec.Parameters,
            commandTimeout: Options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

        var result = await connection.QueryAsync<T>(command)
            .ConfigureAwait(false);

        return result.AsList().AsReadOnly();
    }

    /// <inheritdoc />
    public override async Task<PagedResult<T>> FindPagedAsync(
        ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(spec, nameof(spec));

        var selectSql = BuildSelectSqlWithSoftDelete(spec);
        var countSql = BuildCountSqlWithSoftDelete(spec);

        using var connection = ConnectionFactory.CreateConnection();

        var countCommand = new CommandDefinition(countSql, spec.Parameters,
            commandTimeout: Options.CommandTimeoutSeconds, cancellationToken: cancellationToken);
        var totalCount = await connection.ExecuteScalarAsync<int>(countCommand)
            .ConfigureAwait(false);

        var selectCommand = new CommandDefinition(selectSql, spec.Parameters,
            commandTimeout: Options.CommandTimeoutSeconds, cancellationToken: cancellationToken);
        var items = await connection.QueryAsync<T>(selectCommand)
            .ConfigureAwait(false);

        var page = spec.Skip.HasValue && spec.Take.HasValue && spec.Take.Value > 0
            ? (spec.Skip.Value / spec.Take.Value) + 1
            : 1;
        var pageSize = spec.Take ?? totalCount;

        return new PagedResult<T>(items.AsList().AsReadOnly(), totalCount, page, pageSize);
    }

    /// <inheritdoc />
    public override async Task<int> CountAsync(
        ISpecification<T>? spec = null, CancellationToken cancellationToken = default)
    {
        var countSql = spec is not null
            ? BuildCountSqlWithSoftDelete(spec)
            : $"SELECT COUNT(*) FROM {Metadata.TableName} WHERE is_deleted = FALSE";

        using var connection = ConnectionFactory.CreateConnection();
        var command = new CommandDefinition(countSql, spec?.Parameters,
            commandTimeout: Options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<int>(command)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<int> BulkDeleteAsync(
        IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids, nameof(ids));

        var idList = ids.ToList();
        if (idList.Count == 0) return 0;

        var currentUser = GetCurrentUser();
        var now = DateTime.UtcNow;

        var sql = $"UPDATE {Metadata.TableName} SET is_deleted = TRUE, deleted_at = @DeletedAt, deleted_by = @DeletedBy WHERE id = ANY(@Ids) AND is_deleted = FALSE";

        using var connection = ConnectionFactory.CreateConnection();
        var command = new CommandDefinition(sql,
            new { Ids = idList.ToArray(), DeletedAt = now, DeletedBy = currentUser },
            commandTimeout: Options.CommandTimeoutSeconds,
            cancellationToken: cancellationToken);

        var affected = await connection.ExecuteAsync(command)
            .ConfigureAwait(false);

        Logger.LogInformation(
            "Suppression logique en masse reussie : {Count} entite(s) dans {Table} par {User}.",
            affected, Metadata.TableName, currentUser);

        return affected;
    }

    // ========================================================================
    // Helpers
    // ========================================================================

    private string GetCurrentUser()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";
    }

    private string BuildSelectSqlWithSoftDelete(ISpecification<T> spec)
    {
        var sb = new StringBuilder($"SELECT * FROM {Metadata.TableName} WHERE is_deleted = FALSE");

        if (!string.IsNullOrWhiteSpace(spec.WhereClause))
        {
            sb.Append($" AND {spec.WhereClause}");
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

    private string BuildCountSqlWithSoftDelete(ISpecification<T> spec)
    {
        var sb = new StringBuilder($"SELECT COUNT(*) FROM {Metadata.TableName} WHERE is_deleted = FALSE");

        if (!string.IsNullOrWhiteSpace(spec.WhereClause))
        {
            sb.Append($" AND {spec.WhereClause}");
        }

        return sb.ToString();
    }
}
