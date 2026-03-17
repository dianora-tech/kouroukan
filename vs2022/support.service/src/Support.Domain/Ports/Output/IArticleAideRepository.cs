using GnDapper.Models;
using Support.Domain.Entities;

namespace Support.Domain.Ports.Output;

/// <summary>
/// Repository pour les articles d'aide.
/// </summary>
public interface IArticleAideRepository
{
    Task<ArticleAide?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<ArticleAide>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<ArticleAide>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<ArticleAide> AddAsync(ArticleAide entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(ArticleAide entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<ArticleAide?> GetBySlugAsync(string slug, CancellationToken ct = default);

    Task<IReadOnlyList<ArticleAide>> RechercherFullTextAsync(string query, int limit = 10, CancellationToken ct = default);
    Task<bool> IncrementVuesAsync(int id, CancellationToken ct = default);
    Task<bool> IncrementUtileAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<ArticleAide>> GetTopConsultesAsync(int limit = 10, CancellationToken ct = default);
}
