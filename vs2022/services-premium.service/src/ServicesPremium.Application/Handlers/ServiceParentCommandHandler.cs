using MediatR;
using ServicesPremium.Application.Commands;
using ServicesPremium.Domain.Entities;
using ServicesPremium.Domain.Ports.Input;

namespace ServicesPremium.Application.Handlers;

/// <summary>
/// Handler pour les commandes CRUD de ServiceParent.
/// </summary>
public sealed class ServiceParentCommandHandler :
    IRequestHandler<CreateServiceParentCommand, ServiceParent>,
    IRequestHandler<UpdateServiceParentCommand, bool>,
    IRequestHandler<DeleteServiceParentCommand, bool>
{
    private readonly IServiceParentService _service;

    public ServiceParentCommandHandler(IServiceParentService service)
    {
        _service = service;
    }

    /// <summary>Cree un nouveau service parent.</summary>
    public async Task<ServiceParent> Handle(CreateServiceParentCommand request, CancellationToken ct)
    {
        var entity = new ServiceParent
        {
            TypeId = request.TypeId,
            Name = request.Name,
            Description = request.Description,
            Code = request.Code,
            Tarif = request.Tarif,
            Periodicite = request.Periodicite,
            EstActif = request.EstActif,
            PeriodeEssaiJours = request.PeriodeEssaiJours,
            TarifDegressif = request.TarifDegressif,
            UserId = request.UserId
        };

        return await _service.CreateAsync(entity, ct);
    }

    /// <summary>Met a jour un service parent existant.</summary>
    public async Task<bool> Handle(UpdateServiceParentCommand request, CancellationToken ct)
    {
        var entity = new ServiceParent
        {
            Id = request.Id,
            TypeId = request.TypeId,
            Name = request.Name,
            Description = request.Description,
            Code = request.Code,
            Tarif = request.Tarif,
            Periodicite = request.Periodicite,
            EstActif = request.EstActif,
            PeriodeEssaiJours = request.PeriodeEssaiJours,
            TarifDegressif = request.TarifDegressif,
            UserId = request.UserId
        };

        return await _service.UpdateAsync(entity, ct);
    }

    /// <summary>Supprime un service parent (soft delete).</summary>
    public async Task<bool> Handle(DeleteServiceParentCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct);
    }
}
