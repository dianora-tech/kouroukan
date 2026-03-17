using GnDapper.Models;
using Bde.Domain.Entities;

namespace Bde.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des depenses BDE.
/// </summary>
public interface IDepenseBdeRepository
{
    Task<DepenseBde?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<DepenseBde>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<DepenseBde>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<DepenseBde> AddAsync(DepenseBde entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(DepenseBde entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
}
