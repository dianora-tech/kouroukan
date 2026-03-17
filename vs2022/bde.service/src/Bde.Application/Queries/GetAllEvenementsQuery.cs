using MediatR;
using EvenementEntity = Bde.Domain.Entities.Evenement;

namespace Bde.Application.Queries;

/// <summary>
/// Requete de recuperation de tous les evenements.
/// </summary>
public sealed record GetAllEvenementsQuery() : IRequest<IReadOnlyList<EvenementEntity>>;
