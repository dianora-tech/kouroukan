using GnDapper.Models;
using Bde.Domain.Entities;

namespace Bde.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des associations.
/// </summary>
public interface IAssociationService
{
    Task<Association?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Association>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Association>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Association> CreateAsync(Association entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Association entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
