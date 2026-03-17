using Finances.Application.Queries;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using GnDapper.Models;
using MediatR;

namespace Finances.Application.Handlers;

public sealed class DepenseQueryHandler :
    IRequestHandler<GetDepenseByIdQuery, Depense?>,
    IRequestHandler<GetAllDepensesQuery, IReadOnlyList<Depense>>,
    IRequestHandler<GetPagedDepensesQuery, PagedResult<Depense>>
{
    private readonly IDepenseService _service;

    public DepenseQueryHandler(IDepenseService service)
    {
        _service = service;
    }

    public async Task<Depense?> Handle(GetDepenseByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Depense>> Handle(GetAllDepensesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Depense>> Handle(GetPagedDepensesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
