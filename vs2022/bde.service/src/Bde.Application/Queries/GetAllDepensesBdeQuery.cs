using MediatR;
using DepenseBdeEntity = Bde.Domain.Entities.DepenseBde;

namespace Bde.Application.Queries;

/// <summary>
/// Requete de recuperation de toutes les depenses BDE.
/// </summary>
public sealed record GetAllDepensesBdeQuery() : IRequest<IReadOnlyList<DepenseBdeEntity>>;
