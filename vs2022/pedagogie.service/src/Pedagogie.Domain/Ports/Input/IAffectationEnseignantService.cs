using GnDapper.Models;
using Pedagogie.Domain.Entities;

namespace Pedagogie.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des affectations enseignant.
/// </summary>
public interface IAffectationEnseignantService
{
    Task<AffectationEnseignant?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<AffectationEnseignant>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<AffectationEnseignant>> GetPagedAsync(int page, int pageSize, int? liaisonId, int? classeId, int? matiereId, int? anneeScolaireId, string? orderBy, CancellationToken ct = default);
    Task<AffectationEnseignant> CreateAsync(AffectationEnseignant entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(AffectationEnseignant entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
