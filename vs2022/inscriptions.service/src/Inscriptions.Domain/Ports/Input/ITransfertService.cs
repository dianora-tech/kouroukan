using GnDapper.Models;
using Inscriptions.Domain.Entities;

namespace Inscriptions.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des transferts.
/// </summary>
public interface ITransfertService
{
    Task<Transfert?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Transfert>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Transfert>> GetPagedAsync(int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default);
    Task<Transfert> CreateAsync(Transfert entity, CancellationToken ct = default);
    Task<bool> AcceptAsync(int id, CancellationToken ct = default);
    Task<bool> RejectAsync(int id, CancellationToken ct = default);
    Task<bool> CompleteAsync(int id, CancellationToken ct = default);
}
