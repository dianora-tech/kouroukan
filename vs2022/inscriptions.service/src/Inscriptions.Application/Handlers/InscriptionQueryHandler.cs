using GnDapper.Models;
using Inscriptions.Application.Queries;
using Inscriptions.Domain.Ports.Input;
using MediatR;
using InscriptionEntity = Inscriptions.Domain.Entities.Inscription;

namespace Inscriptions.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les inscriptions.
/// </summary>
public sealed class InscriptionQueryHandler :
    IRequestHandler<GetInscriptionByIdQuery, InscriptionEntity?>,
    IRequestHandler<GetAllInscriptionsQuery, IReadOnlyList<InscriptionEntity>>,
    IRequestHandler<GetPagedInscriptionsQuery, PagedResult<InscriptionEntity>>
{
    private readonly IInscriptionService _service;

    public InscriptionQueryHandler(IInscriptionService service)
    {
        _service = service;
    }

    public async Task<InscriptionEntity?> Handle(GetInscriptionByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<InscriptionEntity>> Handle(GetAllInscriptionsQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<InscriptionEntity>> Handle(GetPagedInscriptionsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
