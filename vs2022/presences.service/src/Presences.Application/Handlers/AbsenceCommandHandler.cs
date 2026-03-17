using Presences.Application.Commands;
using Presences.Domain.Ports.Input;
using MediatR;
using AbsenceEntity = Presences.Domain.Entities.Absence;

namespace Presences.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les absences.
/// </summary>
public sealed class AbsenceCommandHandler :
    IRequestHandler<CreateAbsenceCommand, AbsenceEntity>,
    IRequestHandler<UpdateAbsenceCommand, bool>,
    IRequestHandler<DeleteAbsenceCommand, bool>
{
    private readonly IAbsenceService _service;

    public AbsenceCommandHandler(IAbsenceService service)
    {
        _service = service;
    }

    public async Task<AbsenceEntity> Handle(CreateAbsenceCommand request, CancellationToken ct)
    {
        var entity = new AbsenceEntity
        {
            TypeId = request.TypeId,
            EleveId = request.EleveId,
            AppelId = request.AppelId,
            DateAbsence = request.DateAbsence,
            HeureDebut = request.HeureDebut,
            HeureFin = request.HeureFin,
            EstJustifiee = request.EstJustifiee,
            MotifJustification = request.MotifJustification,
            PieceJointeUrl = request.PieceJointeUrl,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateAbsenceCommand request, CancellationToken ct)
    {
        var entity = new AbsenceEntity
        {
            Id = request.Id,
            TypeId = request.TypeId,
            EleveId = request.EleveId,
            AppelId = request.AppelId,
            DateAbsence = request.DateAbsence,
            HeureDebut = request.HeureDebut,
            HeureFin = request.HeureFin,
            EstJustifiee = request.EstJustifiee,
            MotifJustification = request.MotifJustification,
            PieceJointeUrl = request.PieceJointeUrl,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteAbsenceCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
