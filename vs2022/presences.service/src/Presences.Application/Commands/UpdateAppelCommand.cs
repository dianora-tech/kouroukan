using MediatR;

namespace Presences.Application.Commands;

/// <summary>
/// Commande de mise a jour d'un appel.
/// </summary>
public sealed record UpdateAppelCommand(
    int Id,
    int ClasseId,
    int EnseignantId,
    int? SeanceId,
    DateTime DateAppel,
    TimeSpan HeureAppel,
    bool EstCloture,
    int UserId) : IRequest<bool>;
