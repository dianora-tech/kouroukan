using Finances.Application.Commands;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using MediatR;

namespace Finances.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les paliers familiaux.
/// </summary>
public sealed class PalierFamilialCommandHandler :
    IRequestHandler<CreatePalierFamilialCommand, PalierFamilial>,
    IRequestHandler<DeletePalierFamilialCommand, bool>
{
    private readonly IPalierFamilialService _service;

    public PalierFamilialCommandHandler(IPalierFamilialService service)
    {
        _service = service;
    }

    public async Task<PalierFamilial> Handle(CreatePalierFamilialCommand request, CancellationToken ct)
    {
        var entity = new PalierFamilial
        {
            CompanyId = request.CompanyId,
            RangEnfant = request.RangEnfant,
            ReductionPourcent = request.ReductionPourcent
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeletePalierFamilialCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
