using Communication.Domain.Entities;
using MediatR;

namespace Communication.Application.Queries;

/// <summary>
/// Requete de recuperation d'une annonce par son identifiant.
/// </summary>
public sealed record GetAnnonceByIdQuery(int Id) : IRequest<Annonce?>;
