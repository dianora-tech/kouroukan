using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Queries;

/// <summary>
/// Requete de recuperation d'un dossier d'admission par son identifiant.
/// </summary>
public sealed record GetDossierAdmissionByIdQuery(int Id) : IRequest<DossierAdmission?>;
