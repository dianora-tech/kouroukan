using GnDapper.Connection;
using GnDapper.Models;
using GnDapper.Options;
using GnDapper.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Support.Domain.Entities;
using Support.Domain.Ports.Output;
using Support.Infrastructure.Dtos;
using Support.Infrastructure.Mappers;

namespace Support.Infrastructure.Repositories;

/// <summary>
/// Repository pour les articles d'aide avec recherche full-text PostgreSQL.
/// </summary>
public sealed class ArticleAideRepository : IArticleAideRepository
{
    private readonly AuditRepository<ArticleAideDto> _repo;

    public ArticleAideRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<Repository<ArticleAideDto>> logger,
        IOptions<GnDapperOptions> options,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = new AuditRepository<ArticleAideDto>(connectionFactory, logger, options, httpContextAccessor);
    }

    public async Task<ArticleAide?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct);
        return dto is null ? null : ArticleAideMapper.ToEntity(dto);
    }

    public async Task<IReadOnlyList<ArticleAide>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _repo.GetAllAsync(ct);
        return dtos.Select(ArticleAideMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<PagedResult<ArticleAide>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        var conditions = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            conditions.Add("(titre ILIKE @Search OR contenu ILIKE @Search OR slug ILIKE @Search)");
            parameters["Search"] = $"%{search}%";
        }

        if (typeId.HasValue)
        {
            conditions.Add("type_id = @TypeId");
            parameters["TypeId"] = typeId.Value;
        }

        var where = conditions.Count > 0 ? string.Join(" AND ", conditions) : null;
        var spec = new SimpleSpecification<ArticleAideDto>(
            where,
            parameters.Count > 0 ? parameters : null,
            string.IsNullOrWhiteSpace(orderBy) ? "ordre ASC" : orderBy,
            (page - 1) * pageSize,
            pageSize);

        var result = await _repo.FindPagedAsync(spec, ct);
        var entities = result.Items.Select(ArticleAideMapper.ToEntity).ToList().AsReadOnly();
        return new PagedResult<ArticleAide>(entities, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<ArticleAide> AddAsync(ArticleAide entity, CancellationToken ct = default)
    {
        var dto = ArticleAideMapper.ToDto(entity);
        var created = await _repo.AddAsync(dto, ct);
        return ArticleAideMapper.ToEntity(created);
    }

    public async Task<bool> UpdateAsync(ArticleAide entity, CancellationToken ct = default)
    {
        var dto = ArticleAideMapper.ToDto(entity);
        return await _repo.UpdateAsync(dto, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default) =>
        await _repo.DeleteAsync(id, ct);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
        await _repo.ExistsAsync(id, ct);

    public async Task<ArticleAide?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM support.articles_aides WHERE slug = @Slug AND is_deleted = FALSE";
        var dtos = await _repo.GetWithQueryAsync(sql, new { Slug = slug }, ct);
        var dto = dtos.FirstOrDefault();
        return dto is null ? null : ArticleAideMapper.ToEntity(dto);
    }

    /// <summary>
    /// Recherche full-text PostgreSQL utilisant tsvector/tsquery.
    /// </summary>
    public async Task<IReadOnlyList<ArticleAide>> RechercherFullTextAsync(string query, int limit = 10, CancellationToken ct = default)
    {
        const string sql = @"
            SELECT *, ts_rank(
                to_tsvector('french', COALESCE(titre, '') || ' ' || COALESCE(contenu, '')),
                plainto_tsquery('french', @Query)
            ) AS rank
            FROM support.articles_aides
            WHERE is_deleted = FALSE
              AND est_publie = TRUE
              AND to_tsvector('french', COALESCE(titre, '') || ' ' || COALESCE(contenu, ''))
                  @@ plainto_tsquery('french', @Query)
            ORDER BY rank DESC
            LIMIT @Limit";
        var dtos = await _repo.GetWithQueryAsync(sql, new { Query = query, Limit = limit }, ct);
        return dtos.Select(ArticleAideMapper.ToEntity).ToList().AsReadOnly();
    }

    public async Task<bool> IncrementVuesAsync(int id, CancellationToken ct = default)
    {
        var article = await GetByIdAsync(id, ct);
        if (article is null) return false;
        article.NombreVues++;
        return await UpdateAsync(article, ct);
    }

    public async Task<bool> IncrementUtileAsync(int id, CancellationToken ct = default)
    {
        var article = await GetByIdAsync(id, ct);
        if (article is null) return false;
        article.NombreUtile++;
        return await UpdateAsync(article, ct);
    }

    public async Task<IReadOnlyList<ArticleAide>> GetTopConsultesAsync(int limit = 10, CancellationToken ct = default)
    {
        var sql = "SELECT * FROM support.articles_aides WHERE is_deleted = FALSE AND est_publie = TRUE ORDER BY nombre_vues DESC LIMIT @Limit";
        var dtos = await _repo.GetWithQueryAsync(sql, new { Limit = limit }, ct);
        return dtos.Select(ArticleAideMapper.ToEntity).ToList().AsReadOnly();
    }
}
