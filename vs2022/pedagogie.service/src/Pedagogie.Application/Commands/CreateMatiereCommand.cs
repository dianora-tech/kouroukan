using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de creation d'une matiere.
/// </summary>
public sealed record CreateMatiereCommand(
    string Name,
    string? Description,
    int TypeId,
    string Code) : IRequest<Matiere>;
