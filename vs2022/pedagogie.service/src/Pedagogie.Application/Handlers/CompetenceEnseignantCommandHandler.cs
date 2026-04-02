using Pedagogie.Application.Commands;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les competences enseignant.
/// </summary>
public sealed class CompetenceEnseignantCommandHandler :
    IRequestHandler<CreateCompetenceEnseignantCommand, CompetenceEnseignant>,
    IRequestHandler<DeleteCompetenceEnseignantCommand, bool>
{
    private readonly ICompetenceEnseignantService _service;

    public CompetenceEnseignantCommandHandler(ICompetenceEnseignantService service)
    {
        _service = service;
    }

    public async Task<CompetenceEnseignant> Handle(CreateCompetenceEnseignantCommand request, CancellationToken ct)
    {
        var entity = new CompetenceEnseignant
        {
            UserId = request.UserId,
            MatiereId = request.MatiereId,
            CycleEtude = request.CycleEtude
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteCompetenceEnseignantCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
