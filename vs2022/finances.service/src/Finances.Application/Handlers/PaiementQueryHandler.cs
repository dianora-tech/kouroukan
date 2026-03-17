using Finances.Application.Queries;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using GnDapper.Models;
using MediatR;

namespace Finances.Application.Handlers;

public sealed class PaiementQueryHandler :
    IRequestHandler<GetPaiementByIdQuery, Paiement?>,
    IRequestHandler<GetAllPaiementsQuery, IReadOnlyList<Paiement>>,
    IRequestHandler<GetPagedPaiementsQuery, PagedResult<Paiement>>
{
    private readonly IPaiementService _service;

    public PaiementQueryHandler(IPaiementService service)
    {
        _service = service;
    }

    public async Task<Paiement?> Handle(GetPaiementByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Paiement>> Handle(GetAllPaiementsQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Paiement>> Handle(GetPagedPaiementsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
