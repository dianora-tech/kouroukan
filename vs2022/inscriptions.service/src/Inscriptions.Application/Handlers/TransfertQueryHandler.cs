using GnDapper.Models;
using Inscriptions.Application.Queries;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using MediatR;

namespace Inscriptions.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les transferts.
/// </summary>
public sealed class TransfertQueryHandler :
    IRequestHandler<GetPagedTransfertsQuery, PagedResult<Transfert>>
{
    private readonly ITransfertService _service;

    public TransfertQueryHandler(ITransfertService service)
    {
        _service = service;
    }

    public async Task<PagedResult<Transfert>> Handle(GetPagedTransfertsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.OrderBy, ct).ConfigureAwait(false);
    }
}
