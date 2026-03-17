using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation de toutes les classes.
/// </summary>
public sealed record GetAllClassesQuery() : IRequest<IReadOnlyList<Classe>>;
