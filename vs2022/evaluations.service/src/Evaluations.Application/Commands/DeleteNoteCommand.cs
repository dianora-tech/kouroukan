using MediatR;

namespace Evaluations.Application.Commands;

/// <summary>
/// Commande de suppression d'une note.
/// </summary>
public sealed record DeleteNoteCommand(int Id) : IRequest<bool>;
