using Finances.Application.Queries;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using GnDapper.Models;
using MediatR;

namespace Finances.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les paliers familiaux.
/// </summary>
public sealed class PalierFamilialQueryHandler :
    IRequestHandler<GetPagedPaliersFamiliauxQuery, PagedResult<PalierFamilial>>
{
    private readonly IPalierFamilialService _service;

    public PalierFamilialQueryHandler(IPalierFamilialService service)
    {
        _service = service;
    }

    public async Task<PagedResult<PalierFamilial>> Handle(GetPagedPaliersFamiliauxQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.CompanyId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
