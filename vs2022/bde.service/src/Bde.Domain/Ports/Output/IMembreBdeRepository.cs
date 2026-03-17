using GnDapper.Models;
using Bde.Domain.Entities;

namespace Bde.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des membres BDE.
/// </summary>
public interface IMembreBdeRepository
{
    Task<MembreBde?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<MembreBde>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<MembreBde>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<MembreBde> AddAsync(MembreBde entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(MembreBde entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
