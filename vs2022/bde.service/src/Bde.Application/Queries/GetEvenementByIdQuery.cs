using MediatR;
using EvenementEntity = Bde.Domain.Entities.Evenement;

namespace Bde.Application.Queries;

/// <summary>
/// Requete de recuperation d'un evenement par son identifiant.
/// </summary>
public sealed record GetEvenementByIdQuery(int Id) : IRequest<EvenementEntity?>;
