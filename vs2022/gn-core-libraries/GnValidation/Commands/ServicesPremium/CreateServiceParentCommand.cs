namespace GnValidation.Commands.ServicesPremium;

/// <summary>Commande de creation d'un service parent.</summary>
public record CreateServiceParentCommand(
    string Name,
    string Code,
    decimal Tarif,
    string Periodicite,
    bool EstActif,
    int? PeriodeEssaiJours,
    bool TarifDegressif);
