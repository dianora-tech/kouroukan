using MediatR;

namespace Personnel.Application.Commands;

/// <summary>
/// Commande de suppression d'un enseignant.
/// </summary>
public sealed record DeleteEnseignantCommand(int Id) : IRequest<bool>;
