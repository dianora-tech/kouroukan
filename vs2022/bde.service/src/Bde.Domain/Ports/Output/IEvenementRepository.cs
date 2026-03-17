using GnDapper.Models;
using Bde.Domain.Entities;

namespace Bde.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des evenements.
/// </summary>
public interface IEvenementRepository
{
    Task<Evenement?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Evenement>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Evenement>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Evenement> AddAsync(Evenement entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Evenement entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
}
