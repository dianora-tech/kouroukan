using MediatR;
using ServicesPremium.Domain.Entities;

namespace ServicesPremium.Application.Queries;

/// <summary>
/// Requete pour obtenir toutes les souscriptions.
/// </summary>
public sealed record GetAllSouscriptionsQuery() : IRequest<IReadOnlyList<Souscription>>;
