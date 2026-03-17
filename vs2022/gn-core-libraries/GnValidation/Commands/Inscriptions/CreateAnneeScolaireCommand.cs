namespace GnValidation.Commands.Inscriptions;

/// <summary>Commande de creation d'une annee scolaire.</summary>
public record CreateAnneeScolaireCommand(
    string Libelle,
    DateTime DateDebut,
    DateTime DateFin,
    bool EstActive);
