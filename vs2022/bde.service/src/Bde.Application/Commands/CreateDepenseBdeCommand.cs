using MediatR;
using DepenseBdeEntity = Bde.Domain.Entities.DepenseBde;

namespace Bde.Application.Commands;

/// <summary>
/// Commande de creation d'une depense BDE.
/// </summary>
public sealed record CreateDepenseBdeCommand(
    int TypeId,
    string Name,
    string? Description,
    int AssociationId,
    decimal Montant,
    string Motif,
    string Categorie,
    string StatutValidation,
    int? ValidateurId,
    int UserId) : IRequest<DepenseBdeEntity>;
