using MediatR;

namespace Pedagogie.Application.Commands;

/// <summary>
/// Commande de mise a jour d'une affectation enseignant (activation/desactivation).
/// </summary>
public sealed record UpdateAffectationEnseignantCommand(
    int Id,
    int LiaisonId,
    int ClasseId,
    int MatiereId,
    int AnneeScolaireId,
    bool EstActive) : IRequest<bool>;
