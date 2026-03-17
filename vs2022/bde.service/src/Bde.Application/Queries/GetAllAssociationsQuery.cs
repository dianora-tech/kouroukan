using MediatR;
using AssociationEntity = Bde.Domain.Entities.Association;

namespace Bde.Application.Queries;

/// <summary>
/// Requete de recuperation de toutes les associations.
/// </summary>
public sealed record GetAllAssociationsQuery() : IRequest<IReadOnlyList<AssociationEntity>>;
