using GnDapper.Models;
using Support.Domain.Entities;

namespace Support.Domain.Ports.Input;

/// <summary>
/// Service metier pour la gestion des articles d'aide.
/// </summary>
public interface IArticleAideService
{
    Task<ArticleAide?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<ArticleAide>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<ArticleAide>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<ArticleAide> CreateAsync(ArticleAide entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(ArticleAide entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<ArticleAide>> RechercherFullTextAsync(string query, int limit = 10, CancellationToken ct = default);
    Task<bool> MarquerUtileAsync(int id, CancellationToken ct = default);
    Task<bool> IncrementerVuesAsync(int id, CancellationToken ct = default);
}
