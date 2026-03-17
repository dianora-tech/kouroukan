using GnDapper.Models;
using Pedagogie.Domain.Entities;

namespace Pedagogie.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des classes.
/// </summary>
public interface IClasseService
{
    Task<Classe?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Classe>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Classe>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<Classe> CreateAsync(Classe entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Classe entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
