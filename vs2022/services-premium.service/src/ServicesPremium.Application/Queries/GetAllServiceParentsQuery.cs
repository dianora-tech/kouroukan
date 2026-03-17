using MediatR;
using ServicesPremium.Domain.Entities;

namespace ServicesPremium.Application.Queries;

/// <summary>
/// Requete pour obtenir tous les services parents.
/// </summary>
public sealed record GetAllServiceParentsQuery() : IRequest<IReadOnlyList<ServiceParent>>;
