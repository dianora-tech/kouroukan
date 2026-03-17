using GnDapper.Models;
using Evaluations.Domain.Entities;

namespace Evaluations.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des bulletins.
/// </summary>
public interface IBulletinRepository
{
    Task<Bulletin?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Bulletin>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Bulletin>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<Bulletin> AddAsync(Bulletin entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Bulletin entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<Bulletin?> GetByEleveTrimestreAsync(int eleveId, int trimestre, int anneeScolaireId, CancellationToken ct = default);
}
