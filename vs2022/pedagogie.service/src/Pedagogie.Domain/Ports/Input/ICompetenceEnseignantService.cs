using GnDapper.Models;
using Pedagogie.Domain.Entities;

namespace Pedagogie.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des competences enseignant.
/// </summary>
public interface ICompetenceEnseignantService
{
    Task<CompetenceEnseignant?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<CompetenceEnseignant>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<CompetenceEnseignant>> GetPagedAsync(int page, int pageSize, int? userId, string? cycleEtude, string? orderBy, CancellationToken ct = default);
    Task<CompetenceEnseignant> CreateAsync(CompetenceEnseignant entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
