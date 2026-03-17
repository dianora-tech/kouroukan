using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de suppression d'une matiere.
/// </summary>
public sealed record DeleteMatiereCommand(int Id) : IRequest<bool>;
