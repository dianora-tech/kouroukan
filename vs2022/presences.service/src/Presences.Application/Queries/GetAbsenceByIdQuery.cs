using MediatR;
using AbsenceEntity = Presences.Domain.Entities.Absence;

namespace Presences.Application.Queries;

/// <summary>
/// Requete de recuperation d'une absence par son identifiant.
/// </summary>
public sealed record GetAbsenceByIdQuery(int Id) : IRequest<AbsenceEntity?>;
