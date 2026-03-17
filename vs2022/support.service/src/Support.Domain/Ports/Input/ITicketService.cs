using GnDapper.Models;
using Support.Domain.Entities;

namespace Support.Domain.Ports.Input;

/// <summary>
/// Service metier pour la gestion des tickets de support.
/// </summary>
public interface ITicketService
{
    Task<Ticket?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Ticket>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Ticket>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Ticket> CreateAsync(Ticket entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Ticket entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<ReponseTicket>> GetReponsesAsync(int ticketId, CancellationToken ct = default);
    Task<ReponseTicket> AddReponseAsync(ReponseTicket reponse, CancellationToken ct = default);
}
