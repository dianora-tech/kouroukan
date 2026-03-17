using GnDapper.Models;
using Bde.Domain.Entities;

namespace Bde.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des associations.
/// </summary>
public interface IAssociationRepository
{
    Task<Association?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Association>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Association>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Association> AddAsync(Association entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Association entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
}

public sealed record TypeDto(int Id, string Name, string? Description);
