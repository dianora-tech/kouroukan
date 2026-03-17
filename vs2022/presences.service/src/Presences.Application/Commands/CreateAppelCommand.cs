using MediatR;
using AppelEntity = Presences.Domain.Entities.Appel;

namespace Presences.Application.Commands;

/// <summary>
/// Commande de creation d'un appel.
/// </summary>
public sealed record CreateAppelCommand(
    int ClasseId,
    int EnseignantId,
    int? SeanceId,
    DateTime DateAppel,
    TimeSpan HeureAppel,
    bool EstCloture,
    int UserId) : IRequest<AppelEntity>;
