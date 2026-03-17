using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de suppression d'une salle.
/// </summary>
public sealed record DeleteSalleCommand(int Id) : IRequest<bool>;
