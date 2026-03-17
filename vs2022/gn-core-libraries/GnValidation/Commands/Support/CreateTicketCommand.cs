namespace GnValidation.Commands.Support;

/// <summary>Commande de creation d'un ticket.</summary>
public record CreateTicketCommand(
    int AuteurId,
    string Sujet,
    string Contenu,
    string Priorite,
    string StatutTicket,
    string CategorieTicket,
    string? ModuleConcerne);
