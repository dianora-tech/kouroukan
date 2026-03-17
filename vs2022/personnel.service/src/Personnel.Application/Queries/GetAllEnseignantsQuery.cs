using Personnel.Domain.Entities;
using MediatR;

namespace Personnel.Application.Queries;

/// <summary>
/// Requete de recuperation de tous les enseignants.
/// </summary>
public sealed record GetAllEnseignantsQuery() : IRequest<IReadOnlyList<Enseignant>>;
