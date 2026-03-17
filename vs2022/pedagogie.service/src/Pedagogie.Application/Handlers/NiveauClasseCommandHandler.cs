using Pedagogie.Application.Commands;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les niveaux de classes.
/// </summary>
public sealed class NiveauClasseCommandHandler :
    IRequestHandler<CreateNiveauClasseCommand, NiveauClasse>,
    IRequestHandler<UpdateNiveauClasseCommand, bool>,
    IRequestHandler<DeleteNiveauClasseCommand, bool>
{
    private readonly INiveauClasseService _service;

    public NiveauClasseCommandHandler(INiveauClasseService service)
    {
        _service = service;
    }

    public async Task<NiveauClasse> Handle(CreateNiveauClasseCommand request, CancellationToken ct)
    {
        var entity = new NiveauClasse
        {
            Name = request.Name,
            Description = request.Description,
            TypeId = request.TypeId,
            Code = request.Code,
            Ordre = request.Ordre,
            CycleEtude = request.CycleEtude,
            AgeOfficielEntree = request.AgeOfficielEntree,
            MinistereTutelle = request.MinistereTutelle,
            ExamenSortie = request.ExamenSortie,
            TauxHoraireEnseignant = request.TauxHoraireEnseignant
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateNiveauClasseCommand request, CancellationToken ct)
    {
        var entity = new NiveauClasse
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            TypeId = request.TypeId,
            Code = request.Code,
            Ordre = request.Ordre,
            CycleEtude = request.CycleEtude,
            AgeOfficielEntree = request.AgeOfficielEntree,
            MinistereTutelle = request.MinistereTutelle,
            ExamenSortie = request.ExamenSortie,
            TauxHoraireEnseignant = request.TauxHoraireEnseignant
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteNiveauClasseCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
