using MediatR;
using ServicesPremium.Domain.Entities;

namespace ServicesPremium.Application.Commands;

/// <summary>
/// Commande de creation d'une souscription.
/// </summary>
public sealed record CreateSouscriptionCommand(
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
    int UserId) : IRequest<Souscription>;
