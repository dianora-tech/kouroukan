using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de suppression d'une seance.
/// </summary>
public sealed record DeleteSeanceCommand(int Id) : IRequest<bool>;
