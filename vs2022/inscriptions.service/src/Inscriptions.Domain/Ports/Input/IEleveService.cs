using GnDapper.Models;
using Inscriptions.Domain.Entities;

namespace Inscriptions.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des eleves.
/// </summary>
public interface IEleveService
{
    Task<Eleve?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Eleve>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Eleve>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<Eleve> CreateAsync(Eleve entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Eleve entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
