using MediatR;
using AssociationEntity = Bde.Domain.Entities.Association;

namespace Bde.Application.Commands;

/// <summary>
/// Commande de creation d'une association.
/// </summary>
public sealed record CreateAssociationCommand(
    int TypeId,
    string Name,
    string? Description,
    string? Sigle,
    string AnneeScolaire,
    string Statut,
    decimal BudgetAnnuel,
    int? SuperviseurId,
    int UserId) : IRequest<AssociationEntity>;
