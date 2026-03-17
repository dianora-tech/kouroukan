using GnDapper.Models;
using Pedagogie.Domain.Entities;

namespace Pedagogie.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des seances.
/// </summary>
public interface ISeanceRepository
{
    Task<Seance?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Seance>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Seance>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<Seance> AddAsync(Seance entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Seance entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
