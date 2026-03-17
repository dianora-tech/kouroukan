using MediatR;
using AppelEntity = Presences.Domain.Entities.Appel;

namespace Presences.Application.Queries;

/// <summary>
/// Requete de recuperation de tous les appels.
/// </summary>
public sealed record GetAllAppelsQuery() : IRequest<IReadOnlyList<AppelEntity>>;
