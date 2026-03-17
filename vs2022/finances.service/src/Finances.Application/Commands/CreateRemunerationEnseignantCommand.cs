using Finances.Domain.Entities;
using MediatR;

namespace Finances.Application.Commands;

public sealed record CreateRemunerationEnseignantCommand(
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
    int UserId) : IRequest<RemunerationEnseignant>;
