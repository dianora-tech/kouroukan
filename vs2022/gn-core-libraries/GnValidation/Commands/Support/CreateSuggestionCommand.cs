namespace GnValidation.Commands.Support;

/// <summary>Commande de creation d'une suggestion.</summary>
public record CreateSuggestionCommand(
    int AuteurId,
    string Titre,
    string Contenu,
    string? ModuleConcerne,
    string StatutSuggestion);
