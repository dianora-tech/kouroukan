using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation de toutes les salles.
/// </summary>
public sealed record GetAllSallesQuery() : IRequest<IReadOnlyList<Salle>>;
