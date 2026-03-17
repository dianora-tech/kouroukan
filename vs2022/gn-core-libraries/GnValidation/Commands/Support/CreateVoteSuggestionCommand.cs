namespace GnValidation.Commands.Support;

/// <summary>Commande de creation d'un vote de suggestion.</summary>
public record CreateVoteSuggestionCommand(
    int SuggestionId,
    int VotantId);
