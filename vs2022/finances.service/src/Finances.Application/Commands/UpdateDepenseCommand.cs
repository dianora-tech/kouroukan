using MediatR;

namespace Finances.Application.Commands;

public sealed record UpdateDepenseCommand(
    int Id,
    int TypeId,
    decimal Montant,
    string MotifDepense,
    string Categorie,
    string BeneficiaireNom,
    string? BeneficiaireTelephone,
    string? BeneficiaireNIF,
    string StatutDepense,
    DateTime DateDemande,
    DateTime? DateValidation,
    int? ValidateurId,
    string? PieceJointeUrl,
    string NumeroJustificatif,
    int UserId) : IRequest<bool>;
