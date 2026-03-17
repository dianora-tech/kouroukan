namespace GnValidation.Commands.Finances;

/// <summary>Commande de creation d'un paiement.</summary>
public record CreatePaiementCommand(
    int FactureId,
    decimal MontantPaye,
    DateTime DatePaiement,
    string MoyenPaiement,
    string? ReferenceMobileMoney,
    string StatutPaiement,
    int? CaissierId,
    string NumeroRecu);
