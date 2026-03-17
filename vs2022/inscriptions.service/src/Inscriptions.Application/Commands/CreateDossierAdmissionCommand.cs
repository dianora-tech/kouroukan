using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de creation d'un dossier d'admission.
/// </summary>
public sealed record CreateDossierAdmissionCommand(
    int TypeId,
    int EleveId,
    int AnneeScolaireId,
    string StatutDossier,
    string EtapeActuelle,
    DateTime DateDemande,
    DateTime? DateDecision,
    string? MotifRefus,
    decimal? ScoringInterne,
    string? Commentaires,
    int? ResponsableAdmissionId,
    int UserId) : IRequest<DossierAdmission>;
