using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation d'un niveau de classe par son identifiant.
/// </summary>
public sealed record GetNiveauClasseByIdQuery(int Id) : IRequest<NiveauClasse?>;
