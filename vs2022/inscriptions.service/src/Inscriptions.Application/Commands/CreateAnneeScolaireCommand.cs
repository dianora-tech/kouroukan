using Inscriptions.Domain.Entities;
using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de creation d'une annee scolaire.
/// </summary>
public sealed record CreateAnneeScolaireCommand(
    string Libelle,
    DateTime DateDebut,
    DateTime DateFin,
    bool EstActive) : IRequest<AnneeScolaire>;
