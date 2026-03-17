using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de suppression d'un cahier de textes.
/// </summary>
public sealed record DeleteCahierTextesCommand(int Id) : IRequest<bool>;
