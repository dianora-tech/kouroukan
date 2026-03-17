using GnDapper.Models;
using Evaluations.Application.Queries;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Input;
using MediatR;

namespace Evaluations.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les evaluations.
/// </summary>
public sealed class EvaluationQueryHandler :
    IRequestHandler<GetEvaluationByIdQuery, Evaluation?>,
    IRequestHandler<GetAllEvaluationsQuery, IReadOnlyList<Evaluation>>,
    IRequestHandler<GetPagedEvaluationsQuery, PagedResult<Evaluation>>
{
    private readonly IEvaluationService _service;

    public EvaluationQueryHandler(IEvaluationService service)
    {
        _service = service;
    }

    public async Task<Evaluation?> Handle(GetEvaluationByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Evaluation>> Handle(GetAllEvaluationsQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Evaluation>> Handle(GetPagedEvaluationsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
