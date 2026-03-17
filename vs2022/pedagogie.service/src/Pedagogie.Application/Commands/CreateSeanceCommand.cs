using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de creation d'une seance.
/// </summary>
public sealed record CreateSeanceCommand(
    string Name,
    string? Description,
    int MatiereId,
    int ClasseId,
    int EnseignantId,
    int SalleId,
    int JourSemaine,
    TimeSpan HeureDebut,
    TimeSpan HeureFin,
    int AnneeScolaireId) : IRequest<Seance>;
