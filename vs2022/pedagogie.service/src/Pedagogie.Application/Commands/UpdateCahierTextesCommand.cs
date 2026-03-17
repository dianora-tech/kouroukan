using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de mise a jour d'un cahier de textes.
/// </summary>
public sealed record UpdateCahierTextesCommand(
    int Id,
    string Name,
    string? Description,
    int SeanceId,
    string Contenu,
    DateTime DateSeance,
    string? TravailAFaire) : IRequest<bool>;
