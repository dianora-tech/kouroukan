using GnDapper.Models;
using Pedagogie.Application.Queries;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les cahiers de textes.
/// </summary>
public sealed class CahierTextesQueryHandler :
    IRequestHandler<GetCahierTextesByIdQuery, CahierTextes?>,
    IRequestHandler<GetAllCahiersTextesQuery, IReadOnlyList<CahierTextes>>,
    IRequestHandler<GetPagedCahiersTextesQuery, PagedResult<CahierTextes>>
{
    private readonly ICahierTextesService _service;

    public CahierTextesQueryHandler(ICahierTextesService service)
    {
        _service = service;
    }

    public async Task<CahierTextes?> Handle(GetCahierTextesByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<CahierTextes>> Handle(GetAllCahiersTextesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<CahierTextes>> Handle(GetPagedCahiersTextesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.OrderBy, ct).ConfigureAwait(false);
    }
}
