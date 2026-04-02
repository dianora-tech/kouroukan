using Finances.Domain.Entities;
using MediatR;

namespace Finances.Application.Commands;

/// <summary>
/// Commande de creation d'un moyen de paiement.
/// </summary>
public sealed record CreateMoyenPaiementCommand(
    int CompanyId,
    string Operateur,
    string NumeroCompte,
    string CodeMarchand,
    string Libelle) : IRequest<MoyenPaiement>;
