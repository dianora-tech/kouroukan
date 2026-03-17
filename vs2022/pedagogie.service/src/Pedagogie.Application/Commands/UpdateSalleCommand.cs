using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une salle.
/// </summary>
public sealed record UpdateSalleCommand(
    int Id,
    string Name,
    string? Description,
    int TypeId,
    int Capacite,
    string? Batiment,
    string? Equipements,
    bool EstDisponible) : IRequest<bool>;
