using GnDapper.Models;
using Inscriptions.Application.Queries;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using MediatR;

namespace Inscriptions.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les liaisons parent.
/// </summary>
public sealed class LiaisonParentQueryHandler :
    IRequestHandler<GetPagedLiaisonsParentQuery, PagedResult<LiaisonParent>>
{
    private readonly ILiaisonParentService _service;

    public LiaisonParentQueryHandler(ILiaisonParentService service)
    {
        _service = service;
    }

    public async Task<PagedResult<LiaisonParent>> Handle(GetPagedLiaisonsParentQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.ParentUserId, request.CompanyId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
