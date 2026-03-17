using Finances.Domain.Entities;
using GnDapper.Models;

namespace Finances.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour l'acces aux donnees des factures.
/// </summary>
public interface IFactureRepository
{
    Task<Facture?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Facture>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Facture>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Facture> AddAsync(Facture entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Facture entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<Facture?> GetByNumeroFactureAsync(string numeroFacture, CancellationToken ct = default);
    Task<IReadOnlyList<Facture>> GetByEleveIdAsync(int eleveId, CancellationToken ct = default);
}
