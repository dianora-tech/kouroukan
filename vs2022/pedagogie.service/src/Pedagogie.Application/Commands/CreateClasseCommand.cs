using Pedagogie.Domain.Entities;
using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de creation d'une classe.
/// </summary>
public sealed record CreateClasseCommand(
    string Name,
    string? Description,
    int NiveauClasseId,
    int Capacite,
    int AnneeScolaireId,
    int? EnseignantPrincipalId,
    int Effectif) : IRequest<Classe>;
