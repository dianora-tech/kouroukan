using Pedagogie.Application.Commands;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les matieres.
/// </summary>
public sealed class MatiereCommandHandler :
    IRequestHandler<CreateMatiereCommand, Matiere>,
    IRequestHandler<UpdateMatiereCommand, bool>,
    IRequestHandler<DeleteMatiereCommand, bool>
{
    private readonly IMatiereService _service;

    public MatiereCommandHandler(IMatiereService service)
    {
        _service = service;
    }

    public async Task<Matiere> Handle(CreateMatiereCommand request, CancellationToken ct)
    {
        var entity = new Matiere
        {
            Name = request.Name,
            Description = request.Description,
            TypeId = request.TypeId,
            Code = request.Code,
            Coefficient = request.Coefficient,
            NombreHeures = request.NombreHeures,
            NiveauClasseId = request.NiveauClasseId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateMatiereCommand request, CancellationToken ct)
    {
        var entity = new Matiere
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            TypeId = request.TypeId,
            Code = request.Code,
            Coefficient = request.Coefficient,
            NombreHeures = request.NombreHeures,
            NiveauClasseId = request.NiveauClasseId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteMatiereCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
