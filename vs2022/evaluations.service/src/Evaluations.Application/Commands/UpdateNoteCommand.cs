using MediatR;

namespace Evaluations.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une note.
/// </summary>
public sealed record UpdateNoteCommand(
    int Id,
    string Name,
    string? Description,
    int EvaluationId,
    int EleveId,
    decimal Valeur,
    string? Commentaire,
    DateTime DateSaisie,
    int UserId) : IRequest<bool>;
