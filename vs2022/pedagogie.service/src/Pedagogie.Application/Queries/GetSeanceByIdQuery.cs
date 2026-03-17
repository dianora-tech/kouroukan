using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation d'une seance par son identifiant.
/// </summary>
public sealed record GetSeanceByIdQuery(int Id) : IRequest<Seance?>;
