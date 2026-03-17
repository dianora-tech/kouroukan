using MediatR;

namespace Documents.Application.Commands;

/// <summary>
/// Commande de suppression d'un modele de document.
/// </summary>
public sealed record DeleteModeleDocumentCommand(int Id) : IRequest<bool>;
