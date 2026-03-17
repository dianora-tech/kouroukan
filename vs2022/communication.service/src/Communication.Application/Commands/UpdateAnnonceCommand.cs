using MediatR;

namespace Communication.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une annonce.
/// </summary>
public sealed record UpdateAnnonceCommand(
    int Id,
    string Name,
    string? Description,
    int TypeId,
    string Contenu,
    DateTime DateDebut,
    DateTime? DateFin,
    bool EstActive,
    string CibleAudience,
    int Priorite,
    int UserId) : IRequest<bool>;
