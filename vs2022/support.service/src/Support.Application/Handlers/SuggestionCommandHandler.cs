using MediatR;
using Support.Application.Commands;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;

namespace Support.Application.Handlers;

/// <summary>
/// Handler pour les commandes de suggestions.
/// </summary>
public sealed class SuggestionCommandHandler :
    IRequestHandler<CreateSuggestionCommand, Suggestion>,
    IRequestHandler<UpdateSuggestionCommand, bool>,
    IRequestHandler<DeleteSuggestionCommand, bool>,
    IRequestHandler<VoterSuggestionCommand, bool>,
    IRequestHandler<RetirerVoteSuggestionCommand, bool>
{
    private readonly ISuggestionService _service;

    public SuggestionCommandHandler(ISuggestionService service) => _service = service;

    public async Task<Suggestion> Handle(CreateSuggestionCommand request, CancellationToken ct)
    {
        var entity = new Suggestion
        {
            Name = request.Name,
            Description = request.Description,
            TypeId = request.TypeId,
            AuteurId = request.AuteurId,
            Titre = request.Titre,
            Contenu = request.Contenu,
            ModuleConcerne = request.ModuleConcerne,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct);
    }

    public async Task<bool> Handle(UpdateSuggestionCommand request, CancellationToken ct)
    {
        var entity = new Suggestion
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            TypeId = request.TypeId,
            AuteurId = request.AuteurId,
            Titre = request.Titre,
            Contenu = request.Contenu,
            ModuleConcerne = request.ModuleConcerne,
            StatutSuggestion = request.StatutSuggestion,
            CommentaireAdmin = request.CommentaireAdmin,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct);
    }

    public async Task<bool> Handle(DeleteSuggestionCommand request, CancellationToken ct) =>
        await _service.DeleteAsync(request.Id, ct);

    public async Task<bool> Handle(VoterSuggestionCommand request, CancellationToken ct) =>
        await _service.VoterAsync(request.SuggestionId, request.VotantId, request.UserId, ct);

    public async Task<bool> Handle(RetirerVoteSuggestionCommand request, CancellationToken ct) =>
        await _service.RetirerVoteAsync(request.SuggestionId, request.VotantId, ct);
}
