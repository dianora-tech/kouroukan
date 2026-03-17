using GnDapper.Models;
using Evaluations.Domain.Entities;

namespace Evaluations.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des bulletins.
/// </summary>
public interface IBulletinService
{
    Task<Bulletin?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Bulletin>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Bulletin>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<Bulletin> CreateAsync(Bulletin entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Bulletin entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
