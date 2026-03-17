using GnDapper.Models;
using Presences.Application.Queries;
using Presences.Domain.Ports.Input;
using MediatR;
using AbsenceEntity = Presences.Domain.Entities.Absence;

namespace Presences.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les absences.
/// </summary>
public sealed class AbsenceQueryHandler :
    IRequestHandler<GetAbsenceByIdQuery, AbsenceEntity?>,
    IRequestHandler<GetAllAbsencesQuery, IReadOnlyList<AbsenceEntity>>,
    IRequestHandler<GetPagedAbsencesQuery, PagedResult<AbsenceEntity>>
{
    private readonly IAbsenceService _service;

    public AbsenceQueryHandler(IAbsenceService service)
    {
        _service = service;
    }

    public async Task<AbsenceEntity?> Handle(GetAbsenceByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<AbsenceEntity>> Handle(GetAllAbsencesQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<AbsenceEntity>> Handle(GetPagedAbsencesQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
