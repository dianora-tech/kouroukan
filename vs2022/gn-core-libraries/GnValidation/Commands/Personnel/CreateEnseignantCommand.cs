namespace GnValidation.Commands.Personnel;

/// <summary>Commande de creation d'un enseignant.</summary>
public record CreateEnseignantCommand(
    string Name,
    string Matricule,
    string Specialite,
    DateTime DateEmbauche,
    string ModeRemuneration,
    decimal? MontantForfait,
    string Telephone,
    string? Email,
    string StatutEnseignant,
    int SoldeCongesAnnuel);
