using GnDapper.Models;
using Pedagogie.Application.Queries;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les classes.
/// </summary>
public sealed class ClasseQueryHandler :
    IRequestHandler<GetClasseByIdQuery, Classe?>,
    IRequestHandler<GetAllClassesQuery, IReadOnlyList<Classe>>,
    IRequestHandler<GetPagedClassesQuery, PagedResult<Classe>>
{
    private readonly IClasseService _service;

    public ClasseQueryHandler(IClasseService service)
    {
        _service = service;
    }

    public async Task<Classe?> Handle(GetClasseByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Classe>> Handle(GetAllClassesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Classe>> Handle(GetPagedClassesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.OrderBy, ct).ConfigureAwait(false);
    }
}
