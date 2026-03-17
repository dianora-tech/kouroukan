using MediatR;
using InscriptionEntity = Inscriptions.Domain.Entities.Inscription;

namespace Inscriptions.Application.Queries;

/// <summary>
/// Requete de recuperation de toutes les inscriptions.
/// </summary>
public sealed record GetAllInscriptionsQuery() : IRequest<IReadOnlyList<InscriptionEntity>>;
