using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une classe.
/// </summary>
public sealed record UpdateClasseCommand(
    int Id,
    string Name,
    string? Description,
    int NiveauClasseId,
    int Capacite,
    int AnneeScolaireId,
    int? EnseignantPrincipalId,
    int Effectif) : IRequest<bool>;
