using MediatR;
using MembreBdeEntity = Bde.Domain.Entities.MembreBde;

namespace Bde.Application.Queries;

/// <summary>
/// Requete de recuperation de tous les membres BDE.
/// </summary>
public sealed record GetAllMembresBdeQuery() : IRequest<IReadOnlyList<MembreBdeEntity>>;
