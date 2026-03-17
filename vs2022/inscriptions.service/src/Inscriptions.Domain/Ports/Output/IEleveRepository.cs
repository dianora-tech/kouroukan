using GnDapper.Models;
using Inscriptions.Domain.Entities;

namespace Inscriptions.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des eleves.
/// </summary>
public interface IEleveRepository
{
    Task<Eleve?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Eleve>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Eleve>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<Eleve> AddAsync(Eleve entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Eleve entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<Eleve?> GetByMatriculeAsync(string matricule, CancellationToken ct = default);
}
