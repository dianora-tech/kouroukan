using MediatR;
using AbsenceEntity = Presences.Domain.Entities.Absence;

namespace Presences.Application.Commands;

/// <summary>
/// Commande de creation d'une absence.
/// </summary>
public sealed record CreateAbsenceCommand(
    int TypeId,
    int EleveId,
    int? AppelId,
    DateTime DateAbsence,
    TimeSpan? HeureDebut,
    TimeSpan? HeureFin,
    bool EstJustifiee,
    string? MotifJustification,
    string? PieceJointeUrl,
    int UserId) : IRequest<AbsenceEntity>;
