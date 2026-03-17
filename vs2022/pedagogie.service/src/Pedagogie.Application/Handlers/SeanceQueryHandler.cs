using GnDapper.Models;
using Pedagogie.Application.Queries;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les seances.
/// </summary>
public sealed class SeanceQueryHandler :
    IRequestHandler<GetSeanceByIdQuery, Seance?>,
    IRequestHandler<GetAllSeancesQuery, IReadOnlyList<Seance>>,
    IRequestHandler<GetPagedSeancesQuery, PagedResult<Seance>>
{
    private readonly ISeanceService _service;

    public SeanceQueryHandler(ISeanceService service)
    {
        _service = service;
    }

    public async Task<Seance?> Handle(GetSeanceByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Seance>> Handle(GetAllSeancesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Seance>> Handle(GetPagedSeancesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.OrderBy, ct).ConfigureAwait(false);
    }
}
