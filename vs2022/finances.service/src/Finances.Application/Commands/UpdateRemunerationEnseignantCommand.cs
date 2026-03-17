using MediatR;

namespace Finances.Application.Commands;

public sealed record UpdateRemunerationEnseignantCommand(
    int Id,
    int EnseignantId,
    int Mois,
    int Annee,
    string ModeRemuneration,
    decimal? MontantForfait,
    decimal? NombreHeures,
    decimal? TauxHoraire,
    string StatutPaiement,
    DateTime? DateValidation,
    int? ValidateurId,
    int UserId) : IRequest<bool>;
