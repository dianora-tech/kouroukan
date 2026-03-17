using GnDapper.Models;
using Bde.Application.Queries;
using Bde.Domain.Ports.Input;
using MediatR;
using MembreBdeEntity = Bde.Domain.Entities.MembreBde;

namespace Bde.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les membres BDE.
/// </summary>
public sealed class MembreBdeQueryHandler :
    IRequestHandler<GetMembreBdeByIdQuery, MembreBdeEntity?>,
    IRequestHandler<GetAllMembresBdeQuery, IReadOnlyList<MembreBdeEntity>>,
    IRequestHandler<GetPagedMembresBdeQuery, PagedResult<MembreBdeEntity>>
{
    private readonly IMembreBdeService _service;

    public MembreBdeQueryHandler(IMembreBdeService service)
    {
        _service = service;
    }

    public async Task<MembreBdeEntity?> Handle(GetMembreBdeByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<MembreBdeEntity>> Handle(GetAllMembresBdeQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<MembreBdeEntity>> Handle(GetPagedMembresBdeQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.OrderBy, ct).ConfigureAwait(false);
    }
}
