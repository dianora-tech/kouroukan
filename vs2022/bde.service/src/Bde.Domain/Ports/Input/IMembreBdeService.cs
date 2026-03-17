using GnDapper.Models;
using Bde.Domain.Entities;

namespace Bde.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des membres BDE.
/// </summary>
public interface IMembreBdeService
{
    Task<MembreBde?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<MembreBde>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<MembreBde>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<MembreBde> CreateAsync(MembreBde entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(MembreBde entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
