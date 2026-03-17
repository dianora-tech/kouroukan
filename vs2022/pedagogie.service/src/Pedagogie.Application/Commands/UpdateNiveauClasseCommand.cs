using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de mise a jour d'un niveau de classe.
/// </summary>
public sealed record UpdateNiveauClasseCommand(
    int Id,
    string Name,
    string? Description,
    int TypeId,
    string Code,
    int Ordre,
    string CycleEtude,
    int? AgeOfficielEntree,
    string? MinistereTutelle,
    string? ExamenSortie,
    decimal? TauxHoraireEnseignant) : IRequest<bool>;
