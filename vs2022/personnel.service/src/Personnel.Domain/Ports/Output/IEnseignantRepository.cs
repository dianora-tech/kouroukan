using GnDapper.Models;
using Personnel.Domain.Entities;

namespace Personnel.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des enseignants.
/// </summary>
public interface IEnseignantRepository
{
    Task<Enseignant?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Enseignant>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Enseignant>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Enseignant> AddAsync(Enseignant entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Enseignant entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<Enseignant?> GetByMatriculeAsync(string matricule, CancellationToken ct = default);
}
