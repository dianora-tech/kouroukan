namespace GnValidation.Commands.Support;

/// <summary>Commande de creation d'un message IA.</summary>
public record CreateMessageIACommand(
    int ConversationIAId,
    string Role,
    string Contenu,
    string? ContexteArticlesIds,
    int? TokensUtilises);
