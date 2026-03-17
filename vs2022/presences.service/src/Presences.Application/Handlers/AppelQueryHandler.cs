using GnDapper.Models;
using Presences.Application.Queries;
using Presences.Domain.Ports.Input;
using MediatR;
using AppelEntity = Presences.Domain.Entities.Appel;

namespace Presences.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les appels.
/// </summary>
public sealed class AppelQueryHandler :
    IRequestHandler<GetAppelByIdQuery, AppelEntity?>,
    IRequestHandler<GetAllAppelsQuery, IReadOnlyList<AppelEntity>>,
    IRequestHandler<GetPagedAppelsQuery, PagedResult<AppelEntity>>
{
    private readonly IAppelService _service;

    public AppelQueryHandler(IAppelService service)
    {
        _service = service;
    }

    public async Task<AppelEntity?> Handle(GetAppelByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<AppelEntity>> Handle(GetAllAppelsQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<AppelEntity>> Handle(GetPagedAppelsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
