using GnDapper.Models;
using Bde.Application.Queries;
using Bde.Domain.Ports.Input;
using MediatR;
using AssociationEntity = Bde.Domain.Entities.Association;

namespace Bde.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les associations.
/// </summary>
public sealed class AssociationQueryHandler :
    IRequestHandler<GetAssociationByIdQuery, AssociationEntity?>,
    IRequestHandler<GetAllAssociationsQuery, IReadOnlyList<AssociationEntity>>,
    IRequestHandler<GetPagedAssociationsQuery, PagedResult<AssociationEntity>>
{
    private readonly IAssociationService _service;

    public AssociationQueryHandler(IAssociationService service)
    {
        _service = service;
    }

    public async Task<AssociationEntity?> Handle(GetAssociationByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<AssociationEntity>> Handle(GetAllAssociationsQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<AssociationEntity>> Handle(GetPagedAssociationsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
