using MediatR;

namespace Inscriptions.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une inscription.
/// </summary>
public sealed record UpdateInscriptionCommand(
    int Id,
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
    int UserId) : IRequest<bool>;
