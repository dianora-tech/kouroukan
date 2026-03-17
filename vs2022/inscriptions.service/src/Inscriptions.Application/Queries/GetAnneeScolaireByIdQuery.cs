using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Queries;

/// <summary>
/// Requete de recuperation d'une annee scolaire par son identifiant.
/// </summary>
public sealed record GetAnneeScolaireByIdQuery(int Id) : IRequest<AnneeScolaire?>;
