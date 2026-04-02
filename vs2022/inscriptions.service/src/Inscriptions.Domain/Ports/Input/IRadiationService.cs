using GnDapper.Models;
using Inscriptions.Domain.Entities;

namespace Inscriptions.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des radiations.
/// </summary>
public interface IRadiationService
{
    Task<Radiation?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Radiation>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Radiation>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<Radiation> CreateAsync(Radiation entity, CancellationToken ct = default);
}
