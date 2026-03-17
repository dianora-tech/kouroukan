using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation de tous les niveaux de classes.
/// </summary>
public sealed record GetAllNiveauClassesQuery() : IRequest<IReadOnlyList<NiveauClasse>>;
