using MediatR;

namespace Bde.Application.Commands;

/// <summary>
/// Commande de mise a jour d'un membre BDE.
/// </summary>
public sealed record UpdateMembreBdeCommand(
    int Id,
    string Name,
    string? Description,
    int AssociationId,
    int EleveId,
    string RoleBde,
    DateTime DateAdhesion,
    decimal? MontantCotisation,
    bool EstActif,
    int UserId) : IRequest<bool>;
