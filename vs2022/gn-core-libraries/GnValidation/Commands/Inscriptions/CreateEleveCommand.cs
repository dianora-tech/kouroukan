namespace GnValidation.Commands.Inscriptions;

/// <summary>Commande de creation d'un eleve.</summary>
public record CreateEleveCommand(
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
    string StatutInscription);
