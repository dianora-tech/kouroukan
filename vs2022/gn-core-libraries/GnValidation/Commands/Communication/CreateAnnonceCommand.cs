namespace GnValidation.Commands.Communication;

/// <summary>Commande de creation d'une annonce.</summary>
public record CreateAnnonceCommand(
    string Name,
    string Contenu,
    DateTime DateDebut,
    DateTime? DateFin,
    bool EstActive,
    string CibleAudience,
    int Priorite);
