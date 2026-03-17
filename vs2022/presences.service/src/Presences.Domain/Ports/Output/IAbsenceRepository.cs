using GnDapper.Models;
using Presences.Domain.Entities;

namespace Presences.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour le repository des absences.
/// </summary>
public interface IAbsenceRepository
{
    Task<Absence?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Absence>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Absence>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Absence> AddAsync(Absence entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Absence entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
}

public sealed record TypeDto(int Id, string Name, string? Description);
