using GnDapper.Models;
using Bde.Application.Queries;
using Bde.Domain.Ports.Input;
using MediatR;
using DepenseBdeEntity = Bde.Domain.Entities.DepenseBde;

namespace Bde.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les depenses BDE.
/// </summary>
public sealed class DepenseBdeQueryHandler :
    IRequestHandler<GetDepenseBdeByIdQuery, DepenseBdeEntity?>,
    IRequestHandler<GetAllDepensesBdeQuery, IReadOnlyList<DepenseBdeEntity>>,
    IRequestHandler<GetPagedDepensesBdeQuery, PagedResult<DepenseBdeEntity>>
{
    private readonly IDepenseBdeService _service;

    public DepenseBdeQueryHandler(IDepenseBdeService service)
    {
        _service = service;
    }

    public async Task<DepenseBdeEntity?> Handle(GetDepenseBdeByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<DepenseBdeEntity>> Handle(GetAllDepensesBdeQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<DepenseBdeEntity>> Handle(GetPagedDepensesBdeQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
