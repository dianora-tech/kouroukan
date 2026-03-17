using Evaluations.Domain.Entities;
using MediatR;

namespace Evaluations.Application.Queries;

/// <summary>
/// Requete de recuperation d'un bulletin par son identifiant.
/// </summary>
public sealed record GetBulletinByIdQuery(int Id) : IRequest<Bulletin?>;
