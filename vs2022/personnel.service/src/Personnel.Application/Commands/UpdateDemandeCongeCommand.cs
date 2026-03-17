using MediatR;

namespace Personnel.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une demande de conge.
/// </summary>
public sealed record UpdateDemandeCongeCommand(
    int Id,
    string Name,
    string? Description,
    int EnseignantId,
    DateTime DateDebut,
    DateTime DateFin,
    string Motif,
    string StatutDemande,
    string? PieceJointeUrl,
    string? CommentaireValidateur,
    int? ValidateurId,
    DateTime? DateValidation,
    bool ImpactPaie,
    int TypeId,
    int UserId) : IRequest<bool>;
