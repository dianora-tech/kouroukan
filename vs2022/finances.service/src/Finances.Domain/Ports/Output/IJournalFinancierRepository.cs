using Finances.Domain.Entities;
using GnDapper.Models;

namespace Finances.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour l'acces aux donnees du journal financier.
/// </summary>
public interface IJournalFinancierRepository
{
    Task<JournalFinancier?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<JournalFinancier>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<JournalFinancier>> GetPagedAsync(int page, int pageSize, int? companyId, string? type, string? categorie, DateTime? dateDebut, DateTime? dateFin, string? orderBy, CancellationToken ct = default);
    Task<JournalFinancier> AddAsync(JournalFinancier entity, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
