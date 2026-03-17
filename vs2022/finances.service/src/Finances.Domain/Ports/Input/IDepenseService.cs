using Finances.Domain.Entities;
using GnDapper.Models;

namespace Finances.Domain.Ports.Input;

/// <summary>
/// Service metier pour la gestion des depenses.
/// </summary>
public interface IDepenseService
{
    Task<Depense?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Depense>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Depense>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Depense> CreateAsync(Depense entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Depense entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
