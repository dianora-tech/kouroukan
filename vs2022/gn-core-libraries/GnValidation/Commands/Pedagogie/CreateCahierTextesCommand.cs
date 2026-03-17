namespace GnValidation.Commands.Pedagogie;

/// <summary>Commande de creation d'un cahier de textes.</summary>
public record CreateCahierTextesCommand(
    int SeanceId,
    string Contenu,
    DateTime DateSeance,
    string? TravailAFaire);
