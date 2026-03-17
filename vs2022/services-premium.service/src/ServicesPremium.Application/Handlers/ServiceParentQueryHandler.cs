using GnDapper.Entities;
using GnDapper.Models;
using MediatR;
using ServicesPremium.Application.Queries;
using ServicesPremium.Domain.Entities;
using ServicesPremium.Domain.Ports.Input;

namespace ServicesPremium.Application.Handlers;

/// <summary>
/// Handler pour les requetes de ServiceParent.
/// </summary>
public sealed class ServiceParentQueryHandler :
    IRequestHandler<GetServiceParentByIdQuery, ServiceParent?>,
    IRequestHandler<GetAllServiceParentsQuery, IReadOnlyList<ServiceParent>>,
    IRequestHandler<GetPagedServiceParentsQuery, PagedResult<ServiceParent>>
{
    private readonly IServiceParentService _service;

    public ServiceParentQueryHandler(IServiceParentService service)
    {
        _service = service;
    }

    /// <summary>Obtient un service parent par son identifiant.</summary>
    public async Task<ServiceParent?> Handle(GetServiceParentByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct);
    }

    /// <summary>Obtient tous les services parents.</summary>
    public async Task<IReadOnlyList<ServiceParent>> Handle(GetAllServiceParentsQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct);
    }

    /// <summary>Obtient les services parents avec pagination.</summary>
    public async Task<PagedResult<ServiceParent>> Handle(GetPagedServiceParentsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct);
    }
}
