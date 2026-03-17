using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de creation d'une salle.
/// </summary>
public sealed record CreateSalleCommand(
    string Name,
    string? Description,
    int TypeId,
    int Capacite,
    string? Batiment,
    string? Equipements,
    bool EstDisponible) : IRequest<Salle>;
