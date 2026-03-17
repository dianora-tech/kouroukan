using MediatR;
using MembreBdeEntity = Bde.Domain.Entities.MembreBde;

namespace Bde.Application.Queries;

/// <summary>
/// Requete de recuperation d'un membre BDE par son identifiant.
/// </summary>
public sealed record GetMembreBdeByIdQuery(int Id) : IRequest<MembreBdeEntity?>;
