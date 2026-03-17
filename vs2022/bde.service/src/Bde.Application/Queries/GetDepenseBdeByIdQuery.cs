using MediatR;
using DepenseBdeEntity = Bde.Domain.Entities.DepenseBde;

namespace Bde.Application.Queries;

/// <summary>
/// Requete de recuperation d'une depense BDE par son identifiant.
/// </summary>
public sealed record GetDepenseBdeByIdQuery(int Id) : IRequest<DepenseBdeEntity?>;
