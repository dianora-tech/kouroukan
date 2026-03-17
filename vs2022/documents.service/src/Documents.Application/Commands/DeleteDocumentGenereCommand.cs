using MediatR;

namespace Documents.Application.Commands;

/// <summary>
/// Commande de suppression d'un document genere.
/// </summary>
public sealed record DeleteDocumentGenereCommand(int Id) : IRequest<bool>;
