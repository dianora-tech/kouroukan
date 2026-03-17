using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation d'une matiere par son identifiant.
/// </summary>
public sealed record GetMatiereByIdQuery(int Id) : IRequest<Matiere?>;
