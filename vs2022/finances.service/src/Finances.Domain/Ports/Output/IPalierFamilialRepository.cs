using Finances.Domain.Entities;
using GnDapper.Models;

namespace Finances.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour l'acces aux donnees des paliers familiaux.
/// </summary>
public interface IPalierFamilialRepository
{
    Task<PalierFamilial?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<PalierFamilial>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<PalierFamilial>> GetPagedAsync(int page, int pageSize, int? companyId, string? orderBy, CancellationToken ct = default);
    Task<PalierFamilial> AddAsync(PalierFamilial entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
