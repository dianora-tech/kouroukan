namespace GnValidation.Commands.Communication;

/// <summary>Commande de creation d'un message.</summary>
public record CreateMessageCommand(
    int ExpediteurId,
    int? DestinataireId,
    string Sujet,
    string Contenu,
    string? GroupeDestinataire);
