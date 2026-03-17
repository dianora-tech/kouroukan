using GnDapper.Models;
using Personnel.Application.Queries;
using Personnel.Domain.Entities;
using Personnel.Domain.Ports.Input;
using MediatR;

namespace Personnel.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les enseignants.
/// </summary>
public sealed class EnseignantQueryHandler :
    IRequestHandler<GetEnseignantByIdQuery, Enseignant?>,
    IRequestHandler<GetAllEnseignantsQuery, IReadOnlyList<Enseignant>>,
    IRequestHandler<GetPagedEnseignantsQuery, PagedResult<Enseignant>>
{
    private readonly IEnseignantService _service;

    public EnseignantQueryHandler(IEnseignantService service)
    {
        _service = service;
    }

    public async Task<Enseignant?> Handle(GetEnseignantByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Enseignant>> Handle(GetAllEnseignantsQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Enseignant>> Handle(GetPagedEnseignantsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
