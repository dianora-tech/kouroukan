using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation d'une classe par son identifiant.
/// </summary>
public sealed record GetClasseByIdQuery(int Id) : IRequest<Classe?>;
