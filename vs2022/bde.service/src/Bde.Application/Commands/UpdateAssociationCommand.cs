using MediatR;

namespace Bde.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une association.
/// </summary>
public sealed record UpdateAssociationCommand(
    int Id,
    int TypeId,
    string Name,
    string? Description,
    string? Sigle,
    string AnneeScolaire,
    string Statut,
    decimal BudgetAnnuel,
    int? SuperviseurId,
    int UserId) : IRequest<bool>;
