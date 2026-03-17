using GnDapper.Models;
using Pedagogie.Application.Queries;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les niveaux de classes.
/// </summary>
public sealed class NiveauClasseQueryHandler :
    IRequestHandler<GetNiveauClasseByIdQuery, NiveauClasse?>,
    IRequestHandler<GetAllNiveauClassesQuery, IReadOnlyList<NiveauClasse>>,
    IRequestHandler<GetPagedNiveauClassesQuery, PagedResult<NiveauClasse>>
{
    private readonly INiveauClasseService _service;

    public NiveauClasseQueryHandler(INiveauClasseService service)
    {
        _service = service;
    }

    public async Task<NiveauClasse?> Handle(GetNiveauClasseByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<NiveauClasse>> Handle(GetAllNiveauClassesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<NiveauClasse>> Handle(GetPagedNiveauClassesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
