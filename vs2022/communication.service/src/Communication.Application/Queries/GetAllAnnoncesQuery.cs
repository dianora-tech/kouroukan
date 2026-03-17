using Communication.Domain.Entities;
using MediatR;

namespace Communication.Application.Queries;

/// <summary>
/// Requete de recuperation de toutes les annonces.
/// </summary>
public sealed record GetAllAnnoncesQuery() : IRequest<IReadOnlyList<Annonce>>;
