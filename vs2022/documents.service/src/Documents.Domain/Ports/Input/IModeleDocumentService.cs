using GnDapper.Models;
using Documents.Domain.Entities;

namespace Documents.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des modeles de documents.
/// </summary>
public interface IModeleDocumentService
{
    Task<ModeleDocument?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<ModeleDocument>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<ModeleDocument>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<ModeleDocument> CreateAsync(ModeleDocument entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(ModeleDocument entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
