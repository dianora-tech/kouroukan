using MediatR;
using Support.Domain.Entities;

namespace Support.Application.Commands;

public sealed record CreateSuggestionCommand(
    string Name,
    string? Description,
    int TypeId,
    int AuteurId,
    string Titre,
    string Contenu,
    string? ModuleConcerne,
    int UserId) : IRequest<Suggestion>;

public sealed record UpdateSuggestionCommand(
    int Id,
    string Name,
    string? Description,
    int TypeId,
    int AuteurId,
    string Titre,
    string Contenu,
    string? ModuleConcerne,
    string StatutSuggestion,
    string? CommentaireAdmin,
    int UserId) : IRequest<bool>;

public sealed record DeleteSuggestionCommand(int Id) : IRequest<bool>;

public sealed record VoterSuggestionCommand(
    int SuggestionId,
    int VotantId,
    int UserId) : IRequest<bool>;

public sealed record RetirerVoteSuggestionCommand(
    int SuggestionId,
    int VotantId) : IRequest<bool>;
