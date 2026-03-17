using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une seance.
/// </summary>
public sealed record UpdateSeanceCommand(
    int Id,
    string Name,
    string? Description,
    int MatiereId,
    int ClasseId,
    int EnseignantId,
    int SalleId,
    int JourSemaine,
    TimeSpan HeureDebut,
    TimeSpan HeureFin,
    int AnneeScolaireId) : IRequest<bool>;
