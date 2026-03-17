using GnDapper.Models;
using Pedagogie.Domain.Entities;

namespace Pedagogie.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des seances.
/// </summary>
public interface ISeanceService
{
    Task<Seance?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Seance>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Seance>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<Seance> CreateAsync(Seance entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Seance entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
