using Finances.Application.Queries;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using GnDapper.Models;
using MediatR;

namespace Finances.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les moyens de paiement.
/// </summary>
public sealed class MoyenPaiementQueryHandler :
    IRequestHandler<GetPagedMoyensPaiementQuery, PagedResult<MoyenPaiement>>
{
    private readonly IMoyenPaiementService _service;

    public MoyenPaiementQueryHandler(IMoyenPaiementService service)
    {
        _service = service;
    }

    public async Task<PagedResult<MoyenPaiement>> Handle(GetPagedMoyensPaiementQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.CompanyId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
