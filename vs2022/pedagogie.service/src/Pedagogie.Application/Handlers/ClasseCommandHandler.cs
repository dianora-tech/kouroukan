using Pedagogie.Application.Commands;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les classes.
/// </summary>
public sealed class ClasseCommandHandler :
    IRequestHandler<CreateClasseCommand, Classe>,
    IRequestHandler<UpdateClasseCommand, bool>,
    IRequestHandler<DeleteClasseCommand, bool>
{
    private readonly IClasseService _service;

    public ClasseCommandHandler(IClasseService service)
    {
        _service = service;
    }

    public async Task<Classe> Handle(CreateClasseCommand request, CancellationToken ct)
    {
        var entity = new Classe
        {
            Name = request.Name,
            Description = request.Description,
            NiveauClasseId = request.NiveauClasseId,
            Capacite = request.Capacite,
            AnneeScolaireId = request.AnneeScolaireId,
            EnseignantPrincipalId = request.EnseignantPrincipalId,
            Effectif = request.Effectif
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateClasseCommand request, CancellationToken ct)
    {
        var entity = new Classe
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            NiveauClasseId = request.NiveauClasseId,
            Capacite = request.Capacite,
            AnneeScolaireId = request.AnneeScolaireId,
            EnseignantPrincipalId = request.EnseignantPrincipalId,
            Effectif = request.Effectif
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteClasseCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
