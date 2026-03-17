namespace GnValidation.Commands.Bde;

/// <summary>Commande de creation d'une association.</summary>
public record CreateAssociationCommand(
    string Name,
    string? Sigle,
    string AnneeScolaire,
    string Statut,
    decimal BudgetAnnuel,
    int? SuperviseurId);
