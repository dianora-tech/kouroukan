namespace GnValidation.Commands.Evaluations;

/// <summary>Commande de creation d'un bulletin.</summary>
public record CreateBulletinCommand(
    int EleveId,
    int ClasseId,
    int Trimestre,
    int AnneeScolaireId,
    decimal MoyenneGenerale,
    int? Rang,
    string? Appreciation,
    bool EstPublie);
