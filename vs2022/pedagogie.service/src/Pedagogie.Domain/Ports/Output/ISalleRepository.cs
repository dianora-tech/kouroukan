using GnDapper.Models;
using Pedagogie.Domain.Entities;

namespace Pedagogie.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des salles.
/// </summary>
public interface ISalleRepository
{
    Task<Salle?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Salle>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Salle>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Salle> AddAsync(Salle entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Salle entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
}

public sealed record TypeDto(int Id, string Name, string? Description);
