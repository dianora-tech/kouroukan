using GnDapper.Models;
using Bde.Domain.Entities;

namespace Bde.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des evenements.
/// </summary>
public interface IEvenementService
{
    Task<Evenement?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Evenement>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Evenement>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Evenement> CreateAsync(Evenement entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Evenement entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
