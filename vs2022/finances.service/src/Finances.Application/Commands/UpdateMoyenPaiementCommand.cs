using MediatR;

namespace Finances.Application.Commands;

/// <summary>
/// Commande de mise a jour d'un moyen de paiement.
/// </summary>
public sealed record UpdateMoyenPaiementCommand(
    int Id,
    int CompanyId,
    string Operateur,
    string NumeroCompte,
    string CodeMarchand,
    string Libelle,
    bool EstActif) : IRequest<bool>;
