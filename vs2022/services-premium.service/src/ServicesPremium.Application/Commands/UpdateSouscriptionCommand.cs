using MediatR;

namespace ServicesPremium.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une souscription.
/// </summary>
public sealed record UpdateSouscriptionCommand(
    int Id,
    string Name,
    string? Description,
    int ServiceParentId,
    int ParentId,
    DateTime DateDebut,
    DateTime? DateFin,
    string StatutSouscription,
    decimal MontantPaye,
    bool RenouvellementAuto,
    DateTime? DateProchainRenouvellement,
    int UserId) : IRequest<bool>;
