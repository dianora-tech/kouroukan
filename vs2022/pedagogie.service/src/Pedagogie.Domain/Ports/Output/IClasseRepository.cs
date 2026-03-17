using GnDapper.Models;
using Pedagogie.Domain.Entities;

namespace Pedagogie.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des classes.
/// </summary>
public interface IClasseRepository
{
    Task<Classe?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Classe>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Classe>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<Classe> AddAsync(Classe entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Classe entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
