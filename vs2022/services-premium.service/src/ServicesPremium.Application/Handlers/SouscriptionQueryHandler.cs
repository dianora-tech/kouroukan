using GnDapper.Entities;
using GnDapper.Models;
using MediatR;
using ServicesPremium.Application.Queries;
using ServicesPremium.Domain.Entities;
using ServicesPremium.Domain.Ports.Input;

namespace ServicesPremium.Application.Handlers;

/// <summary>
/// Handler pour les requetes de Souscription.
/// </summary>
public sealed class SouscriptionQueryHandler :
    IRequestHandler<GetSouscriptionByIdQuery, Souscription?>,
    IRequestHandler<GetAllSouscriptionsQuery, IReadOnlyList<Souscription>>,
    IRequestHandler<GetPagedSouscriptionsQuery, PagedResult<Souscription>>
{
    private readonly ISouscriptionService _service;

    public SouscriptionQueryHandler(ISouscriptionService service)
    {
        _service = service;
    }

    /// <summary>Obtient une souscription par son identifiant.</summary>
    public async Task<Souscription?> Handle(GetSouscriptionByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct);
    }

    /// <summary>Obtient toutes les souscriptions.</summary>
    public async Task<IReadOnlyList<Souscription>> Handle(GetAllSouscriptionsQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct);
    }

    /// <summary>Obtient les souscriptions avec pagination.</summary>
    public async Task<PagedResult<Souscription>> Handle(GetPagedSouscriptionsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct);
    }
}
