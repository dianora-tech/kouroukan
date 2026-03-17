using MediatR;

namespace Evaluations.Application.Commands;

/// <summary>
/// Commande de mise a jour d'un bulletin de notes.
/// </summary>
public sealed record UpdateBulletinCommand(
    int Id,
    string Name,
    string? Description,
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
    int UserId) : IRequest<bool>;
