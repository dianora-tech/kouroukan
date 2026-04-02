using GnDapper.Models;
using Pedagogie.Application.Queries;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les competences enseignant.
/// </summary>
public sealed class CompetenceEnseignantQueryHandler :
    IRequestHandler<GetPagedCompetencesEnseignantQuery, PagedResult<CompetenceEnseignant>>
{
    private readonly ICompetenceEnseignantService _service;

    public CompetenceEnseignantQueryHandler(ICompetenceEnseignantService service)
    {
        _service = service;
    }

    public async Task<PagedResult<CompetenceEnseignant>> Handle(GetPagedCompetencesEnseignantQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.UserId, request.CycleEtude, request.OrderBy, ct).ConfigureAwait(false);
    }
}
