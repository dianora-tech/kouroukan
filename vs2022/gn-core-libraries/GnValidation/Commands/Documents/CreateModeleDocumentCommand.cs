namespace GnValidation.Commands.Documents;

/// <summary>Commande de creation d'un modele de document.</summary>
public record CreateModeleDocumentCommand(
    string Name,
    string Code,
    string Contenu,
    string? LogoUrl,
    string? CouleurPrimaire,
    string? TextePiedPage,
    bool EstActif);
