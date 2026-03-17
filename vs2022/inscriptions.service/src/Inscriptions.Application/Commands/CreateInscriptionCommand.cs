using MediatR;
using InscriptionEntity = Inscriptions.Domain.Entities.Inscription;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de creation d'une inscription.
/// </summary>
public sealed record CreateInscriptionCommand(
    int TypeId,
    int EleveId,
    int ClasseId,
    int AnneeScolaireId,
    DateTime DateInscription,
    decimal MontantInscription,
    bool EstPaye,
    bool EstRedoublant,
    string? TypeEtablissement,
    string? SerieBac,
    string StatutInscription,
    int UserId) : IRequest<InscriptionEntity>;
