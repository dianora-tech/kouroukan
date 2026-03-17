using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de suppression d'un eleve.
/// </summary>
public sealed record DeleteEleveCommand(int Id) : IRequest<bool>;
