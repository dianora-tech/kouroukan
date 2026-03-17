using Personnel.Domain.Entities;
using MediatR;

namespace Personnel.Application.Queries;

/// <summary>
/// Requete de recuperation de toutes les demandes de conge.
/// </summary>
public sealed record GetAllDemandesCongesQuery() : IRequest<IReadOnlyList<DemandeConge>>;
