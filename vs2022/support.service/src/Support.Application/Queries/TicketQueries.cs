using GnDapper.Models;
using MediatR;
using Support.Domain.Entities;

namespace Support.Application.Queries;

public sealed record GetTicketByIdQuery(int Id) : IRequest<Ticket?>;

public sealed record GetAllTicketsQuery() : IRequest<IReadOnlyList<Ticket>>;

public sealed record GetPagedTicketsQuery(
    int Page,
    int PageSize,
    string? Search,
    int? TypeId,
    string? OrderBy) : IRequest<PagedResult<Ticket>>;

public sealed record GetReponsesTicketQuery(int TicketId) : IRequest<IReadOnlyList<ReponseTicket>>;
