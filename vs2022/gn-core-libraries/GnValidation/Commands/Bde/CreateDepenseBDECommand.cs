namespace GnValidation.Commands.Bde;

/// <summary>Commande de creation d'une depense BDE.</summary>
public record CreateDepenseBDECommand(
    string Name,
    int AssociationId,
    decimal Montant,
    string Motif,
    string Categorie,
    string StatutValidation);
