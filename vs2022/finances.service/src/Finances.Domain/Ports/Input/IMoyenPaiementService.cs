using Finances.Domain.Entities;
using GnDapper.Models;

namespace Finances.Domain.Ports.Input;

/// <summary>
/// Service metier pour la gestion des moyens de paiement.
/// </summary>
public interface IMoyenPaiementService
{
    Task<MoyenPaiement?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<MoyenPaiement>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<MoyenPaiement>> GetPagedAsync(int page, int pageSize, int? companyId, string? orderBy, CancellationToken ct = default);
    Task<MoyenPaiement> CreateAsync(MoyenPaiement entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(MoyenPaiement entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
