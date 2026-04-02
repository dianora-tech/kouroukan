using GnDapper.Models;
using Inscriptions.Application.Queries;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using MediatR;

namespace Inscriptions.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les radiations.
/// </summary>
public sealed class RadiationQueryHandler :
    IRequestHandler<GetPagedRadiationsQuery, PagedResult<Radiation>>
{
    private readonly IRadiationService _service;

    public RadiationQueryHandler(IRadiationService service)
    {
        _service = service;
    }

    public async Task<PagedResult<Radiation>> Handle(GetPagedRadiationsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.OrderBy, ct).ConfigureAwait(false);
    }
}
