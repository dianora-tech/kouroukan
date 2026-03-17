using Evaluations.Domain.Entities;
using MediatR;

namespace Evaluations.Application.Commands;

/// <summary>
/// Commande de creation d'une note.
/// </summary>
public sealed record CreateNoteCommand(
    string Name,
    string? Description,
    int EvaluationId,
    int EleveId,
    decimal Valeur,
    string? Commentaire,
    DateTime DateSaisie,
    int UserId) : IRequest<Note>;
