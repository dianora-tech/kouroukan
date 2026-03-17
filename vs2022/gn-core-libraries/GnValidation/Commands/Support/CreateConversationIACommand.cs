namespace GnValidation.Commands.Support;

/// <summary>Commande de creation d'une conversation IA.</summary>
public record CreateConversationIACommand(
    int UtilisateurId,
    string? Titre,
    bool EstActive);
