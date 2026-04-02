using Finances.Application.Queries;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using GnDapper.Models;
using MediatR;

namespace Finances.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour le journal financier.
/// </summary>
public sealed class JournalFinancierQueryHandler :
    IRequestHandler<GetPagedJournalFinancierQuery, PagedResult<JournalFinancier>>
{
    private readonly IJournalFinancierService _service;

    public JournalFinancierQueryHandler(IJournalFinancierService service)
    {
        _service = service;
    }

    public async Task<PagedResult<JournalFinancier>> Handle(GetPagedJournalFinancierQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.CompanyId, request.Type, request.Categorie, request.DateDebut, request.DateFin, request.OrderBy, ct).ConfigureAwait(false);
    }
}
