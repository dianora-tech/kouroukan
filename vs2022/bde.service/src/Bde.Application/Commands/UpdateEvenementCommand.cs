using MediatR;

namespace Bde.Application.Commands;

/// <summary>
/// Commande de mise a jour d'un evenement.
/// </summary>
public sealed record UpdateEvenementCommand(
    int Id,
    int TypeId,
    string Name,
    string? Description,
    int AssociationId,
    DateTime DateEvenement,
    string Lieu,
    int? Capacite,
    decimal? TarifEntree,
    int NombreInscrits,
    string StatutEvenement,
    int UserId) : IRequest<bool>;
