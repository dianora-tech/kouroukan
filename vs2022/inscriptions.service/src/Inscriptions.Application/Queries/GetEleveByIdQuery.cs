using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Queries;

/// <summary>
/// Requete de recuperation d'un eleve par son identifiant.
/// </summary>
public sealed record GetEleveByIdQuery(int Id) : IRequest<Eleve?>;
