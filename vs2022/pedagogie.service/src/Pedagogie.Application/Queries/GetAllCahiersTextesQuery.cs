using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation de tous les cahiers de textes.
/// </summary>
public sealed record GetAllCahiersTextesQuery() : IRequest<IReadOnlyList<CahierTextes>>;
