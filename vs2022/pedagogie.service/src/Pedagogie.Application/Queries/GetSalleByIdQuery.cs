using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation d'une salle par son identifiant.
/// </summary>
public sealed record GetSalleByIdQuery(int Id) : IRequest<Salle?>;
