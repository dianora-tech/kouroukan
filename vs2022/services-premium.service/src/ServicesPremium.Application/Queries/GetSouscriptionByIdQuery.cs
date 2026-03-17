using MediatR;
using ServicesPremium.Domain.Entities;

namespace ServicesPremium.Application.Queries;

/// <summary>
/// Requete pour obtenir une souscription par son identifiant.
/// </summary>
public sealed record GetSouscriptionByIdQuery(int Id) : IRequest<Souscription?>;
