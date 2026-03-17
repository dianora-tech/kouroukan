using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Queries;

/// <summary>
/// Requete de recuperation d'un cahier de textes par son identifiant.
/// </summary>
public sealed record GetCahierTextesByIdQuery(int Id) : IRequest<CahierTextes?>;
