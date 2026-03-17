using Personnel.Domain.Entities;
using MediatR;

namespace Personnel.Application.Commands;

/// <summary>
/// Commande de creation d'un enseignant.
/// </summary>
public sealed record CreateEnseignantCommand(
    string Name,
    string? Description,
    string Matricule,
    string Specialite,
    DateTime DateEmbauche,
    string ModeRemuneration,
    decimal? MontantForfait,
    string Telephone,
    string? Email,
    string StatutEnseignant,
    int SoldeCongesAnnuel,
    int TypeId,
    int UserId) : IRequest<Enseignant>;
