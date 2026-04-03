using Evaluations.Domain.Entities;
using MediatR;

namespace Evaluations.Application.Commands;

/// <summary>
/// Commande de creation d'un bulletin de notes.
/// </summary>
public sealed record CreateBulletinCommand(
    int EleveId,
    int ClasseId,
    int Trimestre,
    int AnneeScolaireId,
    decimal MoyenneGenerale,
    int? Rang,
    string? Appreciation,
    bool EstPublie,
    DateTime DateGeneration,
    string? CheminFichierPdf,
    int UserId) : IRequest<Bulletin>;
