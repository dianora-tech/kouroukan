using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Queries;

/// <summary>
/// Requete de recuperation de toutes les annees scolaires.
/// </summary>
public sealed record GetAllAnneeScolairesQuery() : IRequest<IReadOnlyList<AnneeScolaire>>;
