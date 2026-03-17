using GnDapper.Models;
using Pedagogie.Application.Queries;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les matieres.
/// </summary>
public sealed class MatiereQueryHandler :
    IRequestHandler<GetMatiereByIdQuery, Matiere?>,
    IRequestHandler<GetAllMatieresQuery, IReadOnlyList<Matiere>>,
    IRequestHandler<GetPagedMatieresQuery, PagedResult<Matiere>>
{
    private readonly IMatiereService _service;

    public MatiereQueryHandler(IMatiereService service)
    {
        _service = service;
    }

    public async Task<Matiere?> Handle(GetMatiereByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Matiere>> Handle(GetAllMatieresQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Matiere>> Handle(GetPagedMatieresQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
