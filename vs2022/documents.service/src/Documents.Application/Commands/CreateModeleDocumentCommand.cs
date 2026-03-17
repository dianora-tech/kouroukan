using MediatR;
using Documents.Domain.Entities;

namespace Documents.Application.Commands;

/// <summary>
/// Commande de creation d'un modele de document.
/// </summary>
public sealed record CreateModeleDocumentCommand(
    int TypeId,
    string Code,
    string Contenu,
    string? LogoUrl,
    string? CouleurPrimaire,
    string? TextePiedPage,
    bool EstActif,
    int UserId) : IRequest<ModeleDocument>;
