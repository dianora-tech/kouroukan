using Pedagogie.Application.Commands;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using MediatR;

namespace Pedagogie.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les seances.
/// </summary>
public sealed class SeanceCommandHandler :
    IRequestHandler<CreateSeanceCommand, Seance>,
    IRequestHandler<UpdateSeanceCommand, bool>,
    IRequestHandler<DeleteSeanceCommand, bool>
{
    private readonly ISeanceService _service;

    public SeanceCommandHandler(ISeanceService service)
    {
        _service = service;
    }

    public async Task<Seance> Handle(CreateSeanceCommand request, CancellationToken ct)
    {
        var entity = new Seance
        {
            Name = request.Name,
            Description = request.Description,
            MatiereId = request.MatiereId,
            ClasseId = request.ClasseId,
            EnseignantId = request.EnseignantId,
            SalleId = request.SalleId,
            JourSemaine = request.JourSemaine,
            HeureDebut = request.HeureDebut,
            HeureFin = request.HeureFin,
            AnneeScolaireId = request.AnneeScolaireId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateSeanceCommand request, CancellationToken ct)
    {
        var entity = new Seance
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            MatiereId = request.MatiereId,
            ClasseId = request.ClasseId,
            EnseignantId = request.EnseignantId,
            SalleId = request.SalleId,
            JourSemaine = request.JourSemaine,
            HeureDebut = request.HeureDebut,
            HeureFin = request.HeureFin,
            AnneeScolaireId = request.AnneeScolaireId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteSeanceCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
