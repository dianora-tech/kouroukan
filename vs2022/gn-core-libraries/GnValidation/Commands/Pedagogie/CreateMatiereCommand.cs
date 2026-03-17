namespace GnValidation.Commands.Pedagogie;

/// <summary>Commande de creation d'une matiere.</summary>
public record CreateMatiereCommand(
    string Name,
    string Code,
    decimal Coefficient,
    int NombreHeures,
    int NiveauClasseId);
