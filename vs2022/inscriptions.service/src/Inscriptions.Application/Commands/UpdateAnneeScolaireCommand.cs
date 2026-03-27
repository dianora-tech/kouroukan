using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une annee scolaire.
/// </summary>
public sealed record UpdateAnneeScolaireCommand(
    int Id,
    string Libelle,
    DateTime DateDebut,
    DateTime DateFin,
    bool EstActive,
    string? Code = null,
    string? Description = null,
    string Statut = "preparation",
    DateTime? DateRentree = null,
    int NombrePeriodes = 3,
    string TypePeriode = "trimestre") : IRequest<bool>;
