using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Queries;

/// <summary>
/// Requete de recuperation de tous les dossiers d'admission.
/// </summary>
public sealed record GetAllDossierAdmissionsQuery() : IRequest<IReadOnlyList<DossierAdmission>>;
