using GnDapper.Models;
using Inscriptions.Domain.Entities;

namespace Inscriptions.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des transferts.
/// </summary>
public interface ITransfertRepository
{
    Task<Transfert?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Transfert>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Transfert>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<Transfert> AddAsync(Transfert entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Transfert entity, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
