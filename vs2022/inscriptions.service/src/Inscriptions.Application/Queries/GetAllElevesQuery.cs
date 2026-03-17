using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Queries;

/// <summary>
/// Requete de recuperation de tous les eleves.
/// </summary>
public sealed record GetAllElevesQuery() : IRequest<IReadOnlyList<Eleve>>;
