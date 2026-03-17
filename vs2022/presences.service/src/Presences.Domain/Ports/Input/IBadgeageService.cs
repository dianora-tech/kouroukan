using GnDapper.Models;
using Presences.Domain.Entities;

namespace Presences.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour le service metier des badgeages.
/// </summary>
public interface IBadgeageService
{
    Task<Badgeage?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Badgeage>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Badgeage>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Badgeage> CreateAsync(Badgeage entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Badgeage entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
