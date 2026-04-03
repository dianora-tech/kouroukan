using Communication.Domain.Entities;
using MediatR;

namespace Communication.Application.Commands;

/// <summary>
/// Commande de creation d'une annonce.
/// </summary>
public sealed record CreateAnnonceCommand(
    string Name,
    int TypeId,
    string Contenu,
    DateTime DateDebut,
    DateTime? DateFin,
    bool EstActive,
    string CibleAudience,
    int Priorite,
    int UserId) : IRequest<Annonce>;
