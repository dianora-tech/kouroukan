using Finances.Domain.Entities;
using GnDapper.Models;

namespace Finances.Domain.Ports.Input;

/// <summary>
/// Service metier pour la gestion des remunerations des enseignants.
/// </summary>
public interface IRemunerationEnseignantService
{
    Task<RemunerationEnseignant?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<RemunerationEnseignant>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<RemunerationEnseignant>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<RemunerationEnseignant> CreateAsync(RemunerationEnseignant entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(RemunerationEnseignant entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
