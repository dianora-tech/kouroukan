using Finances.Application.Queries;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using GnDapper.Models;
using MediatR;

namespace Finances.Application.Handlers;

public sealed class FactureQueryHandler :
    IRequestHandler<GetFactureByIdQuery, Facture?>,
    IRequestHandler<GetAllFacturesQuery, IReadOnlyList<Facture>>,
    IRequestHandler<GetPagedFacturesQuery, PagedResult<Facture>>
{
    private readonly IFactureService _service;

    public FactureQueryHandler(IFactureService service)
    {
        _service = service;
    }

    public async Task<Facture?> Handle(GetFactureByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Facture>> Handle(GetAllFacturesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Facture>> Handle(GetPagedFacturesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
