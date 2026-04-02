using Finances.Domain.Entities;
using MediatR;

namespace Finances.Application.Commands;

/// <summary>
/// Commande de creation d'une entree dans le journal financier.
/// </summary>
public sealed record CreateJournalFinancierCommand(
    int CompanyId,
    string Type,
    string Categorie,
    decimal Montant,
    string Description,
    string? ReferenceExterne,
    string ModePaiement,
    DateTime? DateOperation,
    int? EleveId,
    int? ParentUserId) : IRequest<JournalFinancier>;
