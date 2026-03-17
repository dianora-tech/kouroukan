namespace GnValidation.Commands.Pedagogie;

/// <summary>Commande de creation d'une salle.</summary>
public record CreateSalleCommand(
    string Name,
    int Capacite,
    string? Batiment,
    string? Equipements,
    bool EstDisponible);
