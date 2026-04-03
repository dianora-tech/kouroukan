using Evaluations.Application.Commands;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Input;
using MediatR;

namespace Evaluations.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les notes.
/// </summary>
public sealed class NoteCommandHandler :
    IRequestHandler<CreateNoteCommand, Note>,
    IRequestHandler<UpdateNoteCommand, bool>,
    IRequestHandler<DeleteNoteCommand, bool>
{
    private readonly INoteService _service;

    public NoteCommandHandler(INoteService service)
    {
        _service = service;
    }

    public async Task<Note> Handle(CreateNoteCommand request, CancellationToken ct)
    {
        var entity = new Note
        {
            EvaluationId = request.EvaluationId,
            EleveId = request.EleveId,
            Valeur = request.Valeur,
            Commentaire = request.Commentaire,
            DateSaisie = request.DateSaisie,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateNoteCommand request, CancellationToken ct)
    {
        var entity = new Note
        {
            Id = request.Id,
            EvaluationId = request.EvaluationId,
            EleveId = request.EleveId,
            Valeur = request.Valeur,
            Commentaire = request.Commentaire,
            DateSaisie = request.DateSaisie,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteNoteCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
