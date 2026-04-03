using Evaluations.Application.Commands;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Input;
using MediatR;

namespace Evaluations.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les evaluations.
/// </summary>
public sealed class EvaluationCommandHandler :
    IRequestHandler<CreateEvaluationCommand, Evaluation>,
    IRequestHandler<UpdateEvaluationCommand, bool>,
    IRequestHandler<DeleteEvaluationCommand, bool>
{
    private readonly IEvaluationService _service;

    public EvaluationCommandHandler(IEvaluationService service)
    {
        _service = service;
    }

    public async Task<Evaluation> Handle(CreateEvaluationCommand request, CancellationToken ct)
    {
        var entity = new Evaluation
        {
            TypeId = request.TypeId,
            MatiereId = request.MatiereId,
            ClasseId = request.ClasseId,
            EnseignantId = request.EnseignantId,
            DateEvaluation = request.DateEvaluation,
            Coefficient = request.Coefficient,
            NoteMaximale = request.NoteMaximale,
            Trimestre = request.Trimestre,
            AnneeScolaireId = request.AnneeScolaireId,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateEvaluationCommand request, CancellationToken ct)
    {
        var entity = new Evaluation
        {
            Id = request.Id,
            TypeId = request.TypeId,
            MatiereId = request.MatiereId,
            ClasseId = request.ClasseId,
            EnseignantId = request.EnseignantId,
            DateEvaluation = request.DateEvaluation,
            Coefficient = request.Coefficient,
            NoteMaximale = request.NoteMaximale,
            Trimestre = request.Trimestre,
            AnneeScolaireId = request.AnneeScolaireId,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteEvaluationCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
