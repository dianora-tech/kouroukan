using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de creation d'une radiation.
/// </summary>
public sealed record CreateRadiationCommand(
    int EleveId,
    int CompanyId,
    string Motif,
    string? Documents) : IRequest<Radiation>;
