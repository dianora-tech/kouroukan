using MediatR;
using EvenementEntity = Bde.Domain.Entities.Evenement;

namespace Bde.Application.Commands;

/// <summary>
/// Commande de creation d'un evenement.
/// </summary>
public sealed record CreateEvenementCommand(
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
    int UserId) : IRequest<EvenementEntity>;
