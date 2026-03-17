using GnDapper.Entities;
using GnDapper.Models;
using ServicesPremium.Domain.Entities;

namespace ServicesPremium.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour l'acces aux donnees des services parents.
/// </summary>
public interface IServiceParentRepository
{
    Task<ServiceParent?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<ServiceParent>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<ServiceParent>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<ServiceParent> AddAsync(ServiceParent entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(ServiceParent entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
}

public sealed record TypeDto(int Id, string Name, string? Description);
