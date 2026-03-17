namespace GnValidation.Commands.ServicesPremium;

/// <summary>Commande de creation d'une souscription.</summary>
public record CreateSouscriptionCommand(
    int ServiceParentId,
    int ParentId,
    DateTime DateDebut,
    DateTime? DateFin,
    string StatutSouscription,
    decimal MontantPaye,
    bool RenouvellementAuto);
