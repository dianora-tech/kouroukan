using MediatR;
using AbsenceEntity = Presences.Domain.Entities.Absence;

namespace Presences.Application.Queries;

/// <summary>
/// Requete de recuperation de toutes les absences.
/// </summary>
public sealed record GetAllAbsencesQuery() : IRequest<IReadOnlyList<AbsenceEntity>>;
