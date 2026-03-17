using Finances.Domain.Entities;
using GnDapper.Models;

namespace Finances.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour l'acces aux donnees des depenses.
/// </summary>
public interface IDepenseRepository
{
    Task<Depense?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Depense>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Depense>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Depense> AddAsync(Depense entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Depense entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<Depense?> GetByNumeroJustificatifAsync(string numeroJustificatif, CancellationToken ct = default);
}
