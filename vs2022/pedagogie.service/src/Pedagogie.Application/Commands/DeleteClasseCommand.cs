using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de suppression d'une classe.
/// </summary>
public sealed record DeleteClasseCommand(int Id) : IRequest<bool>;
