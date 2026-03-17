namespace GnValidation.Commands.Finances;

/// <summary>Commande de creation d'une depense.</summary>
public record CreateDepenseCommand(
    decimal Montant,
    string MotifDepense,
    string Categorie,
    string BeneficiaireNom,
    string? BeneficiaireTelephone,
    string? BeneficiaireNIF,
    string StatutDepense,
    DateTime DateDemande,
    string? PieceJointeUrl,
    string NumeroJustificatif);
