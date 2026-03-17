namespace GnValidation.Commands.Inscriptions;

/// <summary>Commande de creation d'un dossier d'admission.</summary>
public record CreateDossierAdmissionCommand(
    int EleveId,
    int AnneeScolaireId,
    string StatutDossier,
    string EtapeActuelle,
    DateTime DateDemande,
    DateTime? DateDecision,
    string? MotifRefus,
    decimal? ScoringInterne,
    string? Commentaires,
    int? ResponsableAdmissionId);
