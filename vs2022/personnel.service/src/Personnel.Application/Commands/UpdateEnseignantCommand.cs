using MediatR;

namespace Personnel.Application.Commands;

/// <summary>
/// Commande de mise a jour d'un enseignant.
/// </summary>
public sealed record UpdateEnseignantCommand(
    int Id,
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
    int UserId) : IRequest<bool>;
