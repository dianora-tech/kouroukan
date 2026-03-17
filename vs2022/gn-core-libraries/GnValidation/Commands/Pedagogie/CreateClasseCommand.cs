namespace GnValidation.Commands.Pedagogie;

/// <summary>Commande de creation d'une classe.</summary>
public record CreateClasseCommand(
    string Name,
    int NiveauClasseId,
    int Capacite,
    int AnneeScolaireId,
    int? EnseignantPrincipalId);
