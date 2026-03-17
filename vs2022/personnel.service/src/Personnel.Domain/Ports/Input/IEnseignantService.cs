using GnDapper.Models;
using Personnel.Domain.Entities;

namespace Personnel.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des enseignants.
/// </summary>
public interface IEnseignantService
{
    Task<Enseignant?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Enseignant>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Enseignant>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Enseignant> CreateAsync(Enseignant entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Enseignant entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
