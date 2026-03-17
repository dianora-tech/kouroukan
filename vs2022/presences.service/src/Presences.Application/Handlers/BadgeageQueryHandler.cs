using GnDapper.Models;
using Presences.Application.Queries;
using Presences.Domain.Ports.Input;
using MediatR;
using BadgeageEntity = Presences.Domain.Entities.Badgeage;

namespace Presences.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les badgeages.
/// </summary>
public sealed class BadgeageQueryHandler :
    IRequestHandler<GetBadgeageByIdQuery, BadgeageEntity?>,
    IRequestHandler<GetAllBadgeagesQuery, IReadOnlyList<BadgeageEntity>>,
    IRequestHandler<GetPagedBadgeagesQuery, PagedResult<BadgeageEntity>>
{
    private readonly IBadgeageService _service;

    public BadgeageQueryHandler(IBadgeageService service)
    {
        _service = service;
    }

    public async Task<BadgeageEntity?> Handle(GetBadgeageByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<BadgeageEntity>> Handle(GetAllBadgeagesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<BadgeageEntity>> Handle(GetPagedBadgeagesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
