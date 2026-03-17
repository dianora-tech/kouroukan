using MediatR;
using AssociationEntity = Bde.Domain.Entities.Association;

namespace Bde.Application.Queries;

/// <summary>
/// Requete de recuperation d'une association par son identifiant.
/// </summary>
public sealed record GetAssociationByIdQuery(int Id) : IRequest<AssociationEntity?>;
