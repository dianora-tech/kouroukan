using GnDapper.Models;
using Support.Domain.Entities;

namespace Support.Domain.Ports.Output;

/// <summary>
/// Repository pour les tickets de support.
/// </summary>
public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Ticket>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Ticket>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<Ticket> AddAsync(Ticket entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Ticket entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<ReponseTicket>> GetReponsesAsync(int ticketId, CancellationToken ct = default);
    Task<ReponseTicket> AddReponseAsync(ReponseTicket reponse, CancellationToken ct = default);

    Task<int> CountByStatutAsync(string statut, CancellationToken ct = default);
    Task<double> GetTempsMoyenResolutionAsync(CancellationToken ct = default);
    Task<double> GetMoyenneSatisfactionAsync(CancellationToken ct = default);
}
