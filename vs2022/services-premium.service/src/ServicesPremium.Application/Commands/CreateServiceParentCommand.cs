using MediatR;
using ServicesPremium.Domain.Entities;

namespace ServicesPremium.Application.Commands;

/// <summary>
/// Commande de creation d'un service parent.
/// </summary>
public sealed record CreateServiceParentCommand(
    int TypeId,
    string Name,
    string? Description,
    string Code,
    decimal Tarif,
    string Periodicite,
    bool EstActif,
    int? PeriodeEssaiJours,
    bool TarifDegressif,
    int UserId) : IRequest<ServiceParent>;
