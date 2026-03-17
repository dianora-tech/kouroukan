using GnDapper.Models;
using Evaluations.Application.Queries;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Input;
using MediatR;

namespace Evaluations.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les bulletins.
/// </summary>
public sealed class BulletinQueryHandler :
    IRequestHandler<GetBulletinByIdQuery, Bulletin?>,
    IRequestHandler<GetAllBulletinsQuery, IReadOnlyList<Bulletin>>,
    IRequestHandler<GetPagedBulletinsQuery, PagedResult<Bulletin>>
{
    private readonly IBulletinService _service;

    public BulletinQueryHandler(IBulletinService service)
    {
        _service = service;
    }

    public async Task<Bulletin?> Handle(GetBulletinByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Bulletin>> Handle(GetAllBulletinsQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Bulletin>> Handle(GetPagedBulletinsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.OrderBy, ct).ConfigureAwait(false);
    }
}
