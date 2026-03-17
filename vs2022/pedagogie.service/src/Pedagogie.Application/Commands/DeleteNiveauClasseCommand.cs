using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de suppression d'un niveau de classe.
/// </summary>
public sealed record DeleteNiveauClasseCommand(int Id) : IRequest<bool>;
