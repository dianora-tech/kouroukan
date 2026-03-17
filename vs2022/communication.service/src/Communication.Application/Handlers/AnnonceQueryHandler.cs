using GnDapper.Models;
using Communication.Application.Queries;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Input;
using MediatR;

namespace Communication.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les annonces.
/// </summary>
public sealed class AnnonceQueryHandler :
    IRequestHandler<GetAnnonceByIdQuery, Annonce?>,
    IRequestHandler<GetAllAnnoncesQuery, IReadOnlyList<Annonce>>,
    IRequestHandler<GetPagedAnnoncesQuery, PagedResult<Annonce>>
{
    private readonly IAnnonceService _service;

    public AnnonceQueryHandler(IAnnonceService service)
    {
        _service = service;
    }

    public async Task<Annonce?> Handle(GetAnnonceByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Annonce>> Handle(GetAllAnnoncesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Annonce>> Handle(GetPagedAnnoncesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
