using GnDapper.Models;
using Personnel.Application.Queries;
using Personnel.Domain.Entities;
using Personnel.Domain.Ports.Input;
using MediatR;

namespace Personnel.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les demandes de conge.
/// </summary>
public sealed class DemandeCongeQueryHandler :
    IRequestHandler<GetDemandeCongeByIdQuery, DemandeConge?>,
    IRequestHandler<GetAllDemandesCongesQuery, IReadOnlyList<DemandeConge>>,
    IRequestHandler<GetPagedDemandesCongesQuery, PagedResult<DemandeConge>>
{
    private readonly IDemandeCongeService _service;

    public DemandeCongeQueryHandler(IDemandeCongeService service)
    {
        _service = service;
    }

    public async Task<DemandeConge?> Handle(GetDemandeCongeByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<DemandeConge>> Handle(GetAllDemandesCongesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<DemandeConge>> Handle(GetPagedDemandesCongesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
