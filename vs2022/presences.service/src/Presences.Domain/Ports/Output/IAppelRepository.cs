using GnDapper.Models;
using Presences.Domain.Entities;

namespace Presences.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour le repository des appels.
/// </summary>
public interface IAppelRepository
{
    Task<Appel?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Appel>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Appel>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Appel> AddAsync(Appel entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Appel entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
