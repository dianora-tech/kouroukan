using GnDapper.Models;
using Pedagogie.Application.Queries;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les salles.
/// </summary>
public sealed class SalleQueryHandler :
    IRequestHandler<GetSalleByIdQuery, Salle?>,
    IRequestHandler<GetAllSallesQuery, IReadOnlyList<Salle>>,
    IRequestHandler<GetPagedSallesQuery, PagedResult<Salle>>
{
    private readonly ISalleService _service;

    public SalleQueryHandler(ISalleService service)
    {
        _service = service;
    }

    public async Task<Salle?> Handle(GetSalleByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Salle>> Handle(GetAllSallesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Salle>> Handle(GetPagedSallesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
