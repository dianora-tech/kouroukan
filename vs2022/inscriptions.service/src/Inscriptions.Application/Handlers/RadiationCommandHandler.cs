using Inscriptions.Application.Commands;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using MediatR;

namespace Inscriptions.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les radiations.
/// </summary>
public sealed class RadiationCommandHandler :
    IRequestHandler<CreateRadiationCommand, Radiation>
{
    private readonly IRadiationService _service;

    public RadiationCommandHandler(IRadiationService service)
    {
        _service = service;
    }

    public async Task<Radiation> Handle(CreateRadiationCommand request, CancellationToken ct)
    {
        var entity = new Radiation
        {
            EleveId = request.EleveId,
            CompanyId = request.CompanyId,
            Motif = request.Motif,
            Documents = request.Documents
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }
}
