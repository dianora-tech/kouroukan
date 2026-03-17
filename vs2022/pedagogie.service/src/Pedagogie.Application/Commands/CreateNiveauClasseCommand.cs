using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de creation d'un niveau de classe.
/// </summary>
public sealed record CreateNiveauClasseCommand(
    string Name,
    string? Description,
    int TypeId,
    string Code,
    int Ordre,
    string CycleEtude,
    int? AgeOfficielEntree,
    string? MinistereTutelle,
    string? ExamenSortie,
    decimal? TauxHoraireEnseignant) : IRequest<NiveauClasse>;
