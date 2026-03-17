using MediatR;
using MembreBdeEntity = Bde.Domain.Entities.MembreBde;

namespace Bde.Application.Commands;

/// <summary>
/// Commande de creation d'un membre BDE.
/// </summary>
public sealed record CreateMembreBdeCommand(
    string Name,
    string? Description,
    int AssociationId,
    int EleveId,
    string RoleBde,
    DateTime DateAdhesion,
    decimal? MontantCotisation,
    bool EstActif,
    int UserId) : IRequest<MembreBdeEntity>;
