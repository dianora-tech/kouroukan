namespace GnValidation.Commands.Inscriptions;

/// <summary>Commande de creation d'une inscription.</summary>
public record CreateInscriptionCommand(
    int EleveId,
    int ClasseId,
    int AnneeScolaireId,
    DateTime DateInscription,
    decimal MontantInscription,
    bool EstPaye,
    bool EstRedoublant,
    string? TypeEtablissement,
    string? SerieBac,
    string StatutInscription);
