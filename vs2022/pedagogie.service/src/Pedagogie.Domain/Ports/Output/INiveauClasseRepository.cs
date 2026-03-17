using GnDapper.Models;
using Pedagogie.Domain.Entities;

namespace Pedagogie.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des niveaux de classes.
/// </summary>
public interface INiveauClasseRepository
{
    Task<NiveauClasse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<NiveauClasse>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<NiveauClasse>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<NiveauClasse> AddAsync(NiveauClasse entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(NiveauClasse entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
}
