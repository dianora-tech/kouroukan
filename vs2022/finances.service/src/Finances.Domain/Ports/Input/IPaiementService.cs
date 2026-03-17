using Finances.Domain.Entities;
using GnDapper.Models;

namespace Finances.Domain.Ports.Input;

/// <summary>
/// Service metier pour la gestion des paiements.
/// </summary>
public interface IPaiementService
{
    Task<Paiement?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Paiement>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Paiement>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Paiement> CreateAsync(Paiement entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Paiement entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
