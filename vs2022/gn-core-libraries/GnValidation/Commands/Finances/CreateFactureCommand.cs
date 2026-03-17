namespace GnValidation.Commands.Finances;

/// <summary>Commande de creation d'une facture.</summary>
public record CreateFactureCommand(
    int EleveId,
    int? ParentId,
    int AnneeScolaireId,
    decimal MontantTotal,
    decimal MontantPaye,
    DateTime DateEmission,
    DateTime DateEcheance,
    string StatutFacture,
    string NumeroFacture);
