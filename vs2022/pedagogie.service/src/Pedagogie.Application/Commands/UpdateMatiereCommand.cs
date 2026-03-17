using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une matiere.
/// </summary>
public sealed record UpdateMatiereCommand(
    int Id,
    string Name,
    string? Description,
    int TypeId,
    string Code,
    decimal Coefficient,
    int NombreHeures,
    int NiveauClasseId) : IRequest<bool>;
