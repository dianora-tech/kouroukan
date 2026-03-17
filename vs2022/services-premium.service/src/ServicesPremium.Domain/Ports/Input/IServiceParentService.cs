using GnDapper.Entities;
using GnDapper.Models;
using ServicesPremium.Domain.Entities;

namespace ServicesPremium.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des services parents.
/// </summary>
public interface IServiceParentService
{
    Task<ServiceParent?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<ServiceParent>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<ServiceParent>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<ServiceParent> CreateAsync(ServiceParent entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(ServiceParent entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
