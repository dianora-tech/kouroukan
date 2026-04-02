using GnDapper.Models;
using Pedagogie.Application.Queries;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les affectations enseignant.
/// </summary>
public sealed class AffectationEnseignantQueryHandler :
    IRequestHandler<GetPagedAffectationsEnseignantQuery, PagedResult<AffectationEnseignant>>
{
    private readonly IAffectationEnseignantService _service;

    public AffectationEnseignantQueryHandler(IAffectationEnseignantService service)
    {
        _service = service;
    }

    public async Task<PagedResult<AffectationEnseignant>> Handle(GetPagedAffectationsEnseignantQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.LiaisonId, request.ClasseId, request.MatiereId, request.AnneeScolaireId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
