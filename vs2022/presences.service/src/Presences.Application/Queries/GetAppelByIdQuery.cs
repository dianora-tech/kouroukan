using MediatR;
using AppelEntity = Presences.Domain.Entities.Appel;

namespace Presences.Application.Queries;

/// <summary>
/// Requete de recuperation d'un appel par son identifiant.
/// </summary>
public sealed record GetAppelByIdQuery(int Id) : IRequest<AppelEntity?>;
