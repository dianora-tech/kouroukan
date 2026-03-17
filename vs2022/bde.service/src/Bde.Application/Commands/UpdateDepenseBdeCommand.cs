using MediatR;

namespace Bde.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une depense BDE.
/// </summary>
public sealed record UpdateDepenseBdeCommand(
    int Id,
    int TypeId,
    string Name,
    string? Description,
    int AssociationId,
    decimal Montant,
    string Motif,
    string Categorie,
    string StatutValidation,
    int? ValidateurId,
    int UserId) : IRequest<bool>;
