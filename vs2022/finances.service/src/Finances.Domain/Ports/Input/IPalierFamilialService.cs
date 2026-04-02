using Finances.Domain.Entities;
using GnDapper.Models;

namespace Finances.Domain.Ports.Input;

/// <summary>
/// Service metier pour les paliers familiaux.
/// </summary>
public interface IPalierFamilialService
{
    Task<PalierFamilial?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<PalierFamilial>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<PalierFamilial>> GetPagedAsync(int page, int pageSize, int? companyId, string? orderBy, CancellationToken ct = default);
    Task<PalierFamilial> CreateAsync(PalierFamilial entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
