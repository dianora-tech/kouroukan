using Finances.Domain.Entities;
using GnDapper.Models;

namespace Finances.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour l'acces aux donnees des paiements.
/// </summary>
public interface IPaiementRepository
{
    Task<Paiement?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Paiement>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Paiement>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Paiement> AddAsync(Paiement entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Paiement entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<Paiement?> GetByNumeroRecuAsync(string numeroRecu, CancellationToken ct = default);
    Task<IReadOnlyList<Paiement>> GetByFactureIdAsync(int factureId, CancellationToken ct = default);
}
