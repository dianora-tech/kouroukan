using MediatR;

namespace Presences.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une absence.
/// </summary>
public sealed record UpdateAbsenceCommand(
    int Id,
    int TypeId,
    int EleveId,
    int? AppelId,
    DateTime DateAbsence,
    TimeSpan? HeureDebut,
    TimeSpan? HeureFin,
    bool EstJustifiee,
    string? MotifJustification,
    string? PieceJointeUrl,
    int UserId) : IRequest<bool>;
