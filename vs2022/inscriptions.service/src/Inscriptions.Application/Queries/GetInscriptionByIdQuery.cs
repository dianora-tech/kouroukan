using MediatR;
using InscriptionEntity = Inscriptions.Domain.Entities.Inscription;

namespace Inscriptions.Application.Queries;

/// <summary>
/// Requete de recuperation d'une inscription par son identifiant.
/// </summary>
public sealed record GetInscriptionByIdQuery(int Id) : IRequest<InscriptionEntity?>;
