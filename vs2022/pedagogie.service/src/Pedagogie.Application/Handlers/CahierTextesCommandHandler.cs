using Pedagogie.Application.Commands;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les cahiers de textes.
/// </summary>
public sealed class CahierTextesCommandHandler :
    IRequestHandler<CreateCahierTextesCommand, CahierTextes>,
    IRequestHandler<UpdateCahierTextesCommand, bool>,
    IRequestHandler<DeleteCahierTextesCommand, bool>
{
    private readonly ICahierTextesService _service;

    public CahierTextesCommandHandler(ICahierTextesService service)
    {
        _service = service;
    }

    public async Task<CahierTextes> Handle(CreateCahierTextesCommand request, CancellationToken ct)
    {
        var entity = new CahierTextes
        {
            Name = request.Name,
            Description = request.Description,
            SeanceId = request.SeanceId,
            Contenu = request.Contenu,
            DateSeance = request.DateSeance,
            TravailAFaire = request.TravailAFaire
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateCahierTextesCommand request, CancellationToken ct)
    {
        var entity = new CahierTextes
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            SeanceId = request.SeanceId,
            Contenu = request.Contenu,
            DateSeance = request.DateSeance,
            TravailAFaire = request.TravailAFaire
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteCahierTextesCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
