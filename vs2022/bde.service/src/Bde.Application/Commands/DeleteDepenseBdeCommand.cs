using MediatR;

namespace Bde.Application.Commands;

/// <summary>
/// Commande de suppression d'une depense BDE.
/// </summary>
public sealed record DeleteDepenseBdeCommand(int Id) : IRequest<bool>;
