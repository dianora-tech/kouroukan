using MediatR;

namespace Documents.Application.Commands;

/// <summary>
/// Commande de suppression d'une signature electronique.
/// </summary>
public sealed record DeleteSignatureCommand(int Id) : IRequest<bool>;
