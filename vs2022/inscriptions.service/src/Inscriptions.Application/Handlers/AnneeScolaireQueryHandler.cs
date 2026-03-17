using GnDapper.Models;
using Inscriptions.Application.Queries;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using MediatR;

namespace Inscriptions.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les annees scolaires.
/// </summary>
public sealed class AnneeScolaireQueryHandler :
    IRequestHandler<GetAnneeScolaireByIdQuery, AnneeScolaire?>,
    IRequestHandler<GetAllAnneeScolairesQuery, IReadOnlyList<AnneeScolaire>>,
    IRequestHandler<GetPagedAnneeScolairesQuery, PagedResult<AnneeScolaire>>
{
    private readonly IAnneeScolaireService _service;

    public AnneeScolaireQueryHandler(IAnneeScolaireService service)
    {
        _service = service;
    }

    public async Task<AnneeScolaire?> Handle(GetAnneeScolaireByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<AnneeScolaire>> Handle(GetAllAnneeScolairesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<AnneeScolaire>> Handle(GetPagedAnneeScolairesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.OrderBy, ct).ConfigureAwait(false);
    }
}
