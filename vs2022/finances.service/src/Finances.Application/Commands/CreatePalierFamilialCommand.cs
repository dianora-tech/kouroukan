using Finances.Domain.Entities;
using MediatR;

namespace Finances.Application.Commands;

/// <summary>
/// Commande de creation d'un palier familial.
/// </summary>
public sealed record CreatePalierFamilialCommand(
    int CompanyId,
    int RangEnfant,
    decimal ReductionPourcent) : IRequest<PalierFamilial>;
