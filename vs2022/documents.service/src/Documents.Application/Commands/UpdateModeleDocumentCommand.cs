using MediatR;

namespace Documents.Application.Commands;

/// <summary>
/// Commande de mise a jour d'un modele de document.
/// </summary>
public sealed record UpdateModeleDocumentCommand(
    int Id,
    int TypeId,
    string Code,
    string Contenu,
    string? LogoUrl,
    string? CouleurPrimaire,
    string? TextePiedPage,
    bool EstActif,
    int UserId) : IRequest<bool>;
