using GnDapper.Models;
using MediatR;
using Support.Application.Queries;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;

namespace Support.Application.Handlers;

/// <summary>
/// Handler pour les requetes de tickets.
/// </summary>
public sealed class TicketQueryHandler :
    IRequestHandler<GetTicketByIdQuery, Ticket?>,
    IRequestHandler<GetAllTicketsQuery, IReadOnlyList<Ticket>>,
    IRequestHandler<GetPagedTicketsQuery, PagedResult<Ticket>>,
    IRequestHandler<GetReponsesTicketQuery, IReadOnlyList<ReponseTicket>>
{
    private readonly ITicketService _service;

    public TicketQueryHandler(ITicketService service) => _service = service;

    public async Task<Ticket?> Handle(GetTicketByIdQuery request, CancellationToken ct) =>
        await _service.GetByIdAsync(request.Id, ct);

    public async Task<IReadOnlyList<Ticket>> Handle(GetAllTicketsQuery request, CancellationToken ct) =>
        await _service.GetAllAsync(ct);

    public async Task<PagedResult<Ticket>> Handle(GetPagedTicketsQuery request, CancellationToken ct) =>
        await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct);

    public async Task<IReadOnlyList<ReponseTicket>> Handle(GetReponsesTicketQuery request, CancellationToken ct) =>
        await _service.GetReponsesAsync(request.TicketId, ct);
}
