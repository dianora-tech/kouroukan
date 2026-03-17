namespace GnValidation.Commands.Personnel;

/// <summary>Commande de creation d'une demande de conge.</summary>
public record CreateDemandeCongeCommand(
    int EnseignantId,
    DateTime DateDebut,
    DateTime DateFin,
    string Motif,
    string StatutDemande,
    string? PieceJointeUrl,
    bool ImpactPaie);
