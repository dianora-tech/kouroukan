using Personnel.Domain.Entities;
using MediatR;

namespace Personnel.Application.Queries;

/// <summary>
/// Requete de recuperation d'un enseignant par son identifiant.
/// </summary>
public sealed record GetEnseignantByIdQuery(int Id) : IRequest<Enseignant?>;
