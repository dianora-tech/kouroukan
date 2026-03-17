using Finances.Application.Queries;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using GnDapper.Models;
using MediatR;

namespace Finances.Application.Handlers;

public sealed class RemunerationEnseignantQueryHandler :
    IRequestHandler<GetRemunerationEnseignantByIdQuery, RemunerationEnseignant?>,
    IRequestHandler<GetAllRemunerationsEnseignantsQuery, IReadOnlyList<RemunerationEnseignant>>,
    IRequestHandler<GetPagedRemunerationsEnseignantsQuery, PagedResult<RemunerationEnseignant>>
{
    private readonly IRemunerationEnseignantService _service;

    public RemunerationEnseignantQueryHandler(IRemunerationEnseignantService service)
    {
        _service = service;
    }

    public async Task<RemunerationEnseignant?> Handle(GetRemunerationEnseignantByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<RemunerationEnseignant>> Handle(GetAllRemunerationsEnseignantsQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<RemunerationEnseignant>> Handle(GetPagedRemunerationsEnseignantsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.OrderBy, ct).ConfigureAwait(false);
    }
}
