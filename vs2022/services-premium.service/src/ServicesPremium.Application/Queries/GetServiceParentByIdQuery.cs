using MediatR;
using ServicesPremium.Domain.Entities;

namespace ServicesPremium.Application.Queries;

/// <summary>
/// Requete pour obtenir un service parent par son identifiant.
/// </summary>
public sealed record GetServiceParentByIdQuery(int Id) : IRequest<ServiceParent?>;
