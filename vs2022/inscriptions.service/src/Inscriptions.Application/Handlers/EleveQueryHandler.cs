using GnDapper.Models;
using Inscriptions.Application.Queries;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using MediatR;

namespace Inscriptions.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les eleves.
/// </summary>
public sealed class EleveQueryHandler :
    IRequestHandler<GetEleveByIdQuery, Eleve?>,
    IRequestHandler<GetAllElevesQuery, IReadOnlyList<Eleve>>,
    IRequestHandler<GetPagedElevesQuery, PagedResult<Eleve>>
{
    private readonly IEleveService _service;

    public EleveQueryHandler(IEleveService service)
    {
        _service = service;
    }

    public async Task<Eleve?> Handle(GetEleveByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Eleve>> Handle(GetAllElevesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Eleve>> Handle(GetPagedElevesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.OrderBy, ct).ConfigureAwait(false);
    }
}
