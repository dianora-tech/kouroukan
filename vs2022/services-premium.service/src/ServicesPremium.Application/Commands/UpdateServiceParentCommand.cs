using MediatR;

namespace ServicesPremium.Application.Commands;

/// <summary>
/// Commande de mise a jour d'un service parent.
/// </summary>
public sealed record UpdateServiceParentCommand(
    int Id,
    int TypeId,
    string Name,
    string? Description,
    string Code,
    decimal Tarif,
    string Periodicite,
    bool EstActif,
    int? PeriodeEssaiJours,
    bool TarifDegressif,
    int UserId) : IRequest<bool>;
