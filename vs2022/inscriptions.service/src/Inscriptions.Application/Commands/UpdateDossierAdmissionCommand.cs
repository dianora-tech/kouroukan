using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de mise a jour d'un dossier d'admission.
/// </summary>
public sealed record UpdateDossierAdmissionCommand(
    int Id,
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
    int UserId) : IRequest<bool>;
