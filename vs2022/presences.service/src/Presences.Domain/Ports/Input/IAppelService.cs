using GnDapper.Models;
using Presences.Domain.Entities;

namespace Presences.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour le service metier des appels.
/// </summary>
public interface IAppelService
{
    Task<Appel?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Appel>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Appel>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Appel> CreateAsync(Appel entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Appel entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
