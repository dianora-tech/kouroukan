using GnDapper.Models;
using Pedagogie.Domain.Entities;

namespace Pedagogie.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des affectations enseignant.
/// </summary>
public interface IAffectationEnseignantRepository
{
    Task<AffectationEnseignant?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<AffectationEnseignant>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<AffectationEnseignant>> GetPagedAsync(int page, int pageSize, int? liaisonId, int? classeId, int? matiereId, int? anneeScolaireId, string? orderBy, CancellationToken ct = default);
    Task<AffectationEnseignant> AddAsync(AffectationEnseignant entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(AffectationEnseignant entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
