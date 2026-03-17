using GnDapper.Models;
using Pedagogie.Domain.Entities;

namespace Pedagogie.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des niveaux de classes.
/// </summary>
public interface INiveauClasseService
{
    Task<NiveauClasse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<NiveauClasse>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<NiveauClasse>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<NiveauClasse> CreateAsync(NiveauClasse entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(NiveauClasse entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
