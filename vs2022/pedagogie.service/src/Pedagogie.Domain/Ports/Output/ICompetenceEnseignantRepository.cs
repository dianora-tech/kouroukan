using GnDapper.Models;
using Pedagogie.Domain.Entities;

namespace Pedagogie.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des competences enseignant.
/// </summary>
public interface ICompetenceEnseignantRepository
{
    Task<CompetenceEnseignant?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<CompetenceEnseignant>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<CompetenceEnseignant>> GetPagedAsync(int page, int pageSize, int? userId, string? cycleEtude, string? orderBy, CancellationToken ct = default);
    Task<CompetenceEnseignant> AddAsync(CompetenceEnseignant entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
