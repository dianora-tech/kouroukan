namespace GnValidation.Commands.Bde;

/// <summary>Commande de creation d'un membre BDE.</summary>
public record CreateMembreBDECommand(
    int AssociationId,
    int EleveId,
    string RoleBDE,
    DateTime DateAdhesion,
    decimal? MontantCotisation,
    bool EstActif);
