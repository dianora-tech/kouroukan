using GnDapper.Models;
using Pedagogie.Domain.Entities;

namespace Pedagogie.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des cahiers de textes.
/// </summary>
public interface ICahierTextesRepository
{
    Task<CahierTextes?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<CahierTextes>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<CahierTextes>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<CahierTextes> AddAsync(CahierTextes entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(CahierTextes entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
