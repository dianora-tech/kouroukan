using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de mise a jour d'un eleve.
/// </summary>
public sealed record UpdateEleveCommand(
    int Id,
    string FirstName,
    string LastName,
    DateTime DateNaissance,
    string LieuNaissance,
    string Genre,
    string Nationalite,
    string? Adresse,
    string? PhotoUrl,
    string NumeroMatricule,
    int NiveauClasseId,
    int? ClasseId,
    int? ParentId,
    string StatutInscription,
    int UserId) : IRequest<bool>;
