using GnDapper.Models;
using Bde.Domain.Entities;

namespace Bde.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des depenses BDE.
/// </summary>
public interface IDepenseBdeService
{
    Task<DepenseBde?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<DepenseBde>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<DepenseBde>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<DepenseBde> CreateAsync(DepenseBde entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(DepenseBde entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
