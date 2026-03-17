namespace GnValidation.Commands.Support;

/// <summary>Commande de creation d'une reponse de ticket.</summary>
public record CreateReponseTicketCommand(
    int TicketId,
    int AuteurId,
    string Contenu,
    bool EstReponseIA,
    bool EstInterne);
