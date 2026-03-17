using GnDapper.Models;
using Pedagogie.Domain.Entities;

namespace Pedagogie.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des salles.
/// </summary>
public interface ISalleService
{
    Task<Salle?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Salle>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Salle>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Salle> CreateAsync(Salle entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Salle entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
