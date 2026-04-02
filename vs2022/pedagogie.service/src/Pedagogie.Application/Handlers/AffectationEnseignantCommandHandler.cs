using Pedagogie.Application.Commands;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les affectations enseignant.
/// </summary>
public sealed class AffectationEnseignantCommandHandler :
    IRequestHandler<CreateAffectationEnseignantCommand, AffectationEnseignant>,
    IRequestHandler<UpdateAffectationEnseignantCommand, bool>,
    IRequestHandler<DeleteAffectationEnseignantCommand, bool>
{
    private readonly IAffectationEnseignantService _service;

    public AffectationEnseignantCommandHandler(IAffectationEnseignantService service)
    {
        _service = service;
    }

    public async Task<AffectationEnseignant> Handle(CreateAffectationEnseignantCommand request, CancellationToken ct)
    {
        var entity = new AffectationEnseignant
        {
            LiaisonId = request.LiaisonId,
            ClasseId = request.ClasseId,
            MatiereId = request.MatiereId,
            AnneeScolaireId = request.AnneeScolaireId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateAffectationEnseignantCommand request, CancellationToken ct)
    {
        var entity = new AffectationEnseignant
        {
            Id = request.Id,
            LiaisonId = request.LiaisonId,
            ClasseId = request.ClasseId,
            MatiereId = request.MatiereId,
            AnneeScolaireId = request.AnneeScolaireId,
            EstActive = request.EstActive
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteAffectationEnseignantCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
