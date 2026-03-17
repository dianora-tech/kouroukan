using Finances.Domain.Entities;
using GnDapper.Models;

namespace Finances.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour l'acces aux donnees des remunerations enseignants.
/// </summary>
public interface IRemunerationEnseignantRepository
{
    Task<RemunerationEnseignant?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<RemunerationEnseignant>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<RemunerationEnseignant>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<RemunerationEnseignant> AddAsync(RemunerationEnseignant entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(RemunerationEnseignant entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<RemunerationEnseignant?> GetByEnseignantMoisAnneeAsync(int enseignantId, int mois, int annee, CancellationToken ct = default);
    Task<IReadOnlyList<RemunerationEnseignant>> GetByEnseignantIdAsync(int enseignantId, CancellationToken ct = default);
}
