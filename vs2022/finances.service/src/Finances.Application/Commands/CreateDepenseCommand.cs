using Finances.Domain.Entities;
using MediatR;

namespace Finances.Application.Commands;

public sealed record CreateDepenseCommand(
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
    int UserId) : IRequest<Depense>;
