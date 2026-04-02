using GnDapper.Models;
using Inscriptions.Domain.Entities;

namespace Inscriptions.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des radiations.
/// </summary>
public interface IRadiationRepository
{
    Task<Radiation?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Radiation>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Radiation>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<Radiation> AddAsync(Radiation entity, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
