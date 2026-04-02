using Finances.Domain.Entities;
using GnDapper.Models;

namespace Finances.Domain.Ports.Input;

/// <summary>
/// Service metier pour le journal financier.
/// </summary>
public interface IJournalFinancierService
{
    Task<JournalFinancier?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<JournalFinancier>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<JournalFinancier>> GetPagedAsync(int page, int pageSize, int? companyId, string? type, string? categorie, DateTime? dateDebut, DateTime? dateFin, string? orderBy, CancellationToken ct = default);
    Task<JournalFinancier> CreateAsync(JournalFinancier entity, CancellationToken ct = default);
}
