using GnDapper.Models;
using Presences.Domain.Entities;

namespace Presences.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour le service metier des absences.
/// </summary>
public interface IAbsenceService
{
    Task<Absence?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Absence>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Absence>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Absence> CreateAsync(Absence entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Absence entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
