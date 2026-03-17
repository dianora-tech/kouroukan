namespace GnValidation.Commands.Communication;

/// <summary>Commande de creation d'une notification.</summary>
public record CreateNotificationCommand(
    string Name,
    string DestinatairesIds,
    string Contenu,
    string Canal,
    string? LienAction);
