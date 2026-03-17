using GnDapper.Models;
using Presences.Domain.Entities;

namespace Presences.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour le repository des badgeages.
/// </summary>
public interface IBadgeageRepository
{
    Task<Badgeage?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Badgeage>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Badgeage>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Badgeage> AddAsync(Badgeage entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Badgeage entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
}
