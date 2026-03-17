namespace GnValidation.Commands.Bde;

/// <summary>Commande de creation d'un evenement.</summary>
public record CreateEvenementCommand(
    string Name,
    int AssociationId,
    DateTime DateEvenement,
    string Lieu,
    int? Capacite,
    decimal? TarifEntree,
    string StatutEvenement);
