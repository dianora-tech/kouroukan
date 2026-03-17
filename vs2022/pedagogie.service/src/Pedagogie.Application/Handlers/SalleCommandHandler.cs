using Pedagogie.Application.Commands;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les salles.
/// </summary>
public sealed class SalleCommandHandler :
    IRequestHandler<CreateSalleCommand, Salle>,
    IRequestHandler<UpdateSalleCommand, bool>,
    IRequestHandler<DeleteSalleCommand, bool>
{
    private readonly ISalleService _service;

    public SalleCommandHandler(ISalleService service)
    {
        _service = service;
    }

    public async Task<Salle> Handle(CreateSalleCommand request, CancellationToken ct)
    {
        var entity = new Salle
        {
            Name = request.Name,
            Description = request.Description,
            TypeId = request.TypeId,
            Capacite = request.Capacite,
            Batiment = request.Batiment,
            Equipements = request.Equipements,
            EstDisponible = request.EstDisponible
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateSalleCommand request, CancellationToken ct)
    {
        var entity = new Salle
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            TypeId = request.TypeId,
            Capacite = request.Capacite,
            Batiment = request.Batiment,
            Equipements = request.Equipements,
            EstDisponible = request.EstDisponible
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteSalleCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
