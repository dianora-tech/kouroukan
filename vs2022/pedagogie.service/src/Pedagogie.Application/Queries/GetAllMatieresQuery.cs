using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation de toutes les matieres.
/// </summary>
public sealed record GetAllMatieresQuery() : IRequest<IReadOnlyList<Matiere>>;
