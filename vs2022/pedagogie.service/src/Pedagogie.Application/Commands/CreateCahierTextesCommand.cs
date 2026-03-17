using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de creation d'un cahier de textes.
/// </summary>
public sealed record CreateCahierTextesCommand(
    string Name,
    string? Description,
    int SeanceId,
    string Contenu,
    DateTime DateSeance,
    string? TravailAFaire) : IRequest<CahierTextes>;
