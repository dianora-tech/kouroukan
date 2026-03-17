using GnDapper.Models;
using Bde.Application.Queries;
using Bde.Domain.Ports.Input;
using MediatR;
using EvenementEntity = Bde.Domain.Entities.Evenement;

namespace Bde.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les evenements.
/// </summary>
public sealed class EvenementQueryHandler :
    IRequestHandler<GetEvenementByIdQuery, EvenementEntity?>,
    IRequestHandler<GetAllEvenementsQuery, IReadOnlyList<EvenementEntity>>,
    IRequestHandler<GetPagedEvenementsQuery, PagedResult<EvenementEntity>>
{
    private readonly IEvenementService _service;

    public EvenementQueryHandler(IEvenementService service)
    {
        _service = service;
    }

    public async Task<EvenementEntity?> Handle(GetEvenementByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<EvenementEntity>> Handle(GetAllEvenementsQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<EvenementEntity>> Handle(GetPagedEvenementsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
