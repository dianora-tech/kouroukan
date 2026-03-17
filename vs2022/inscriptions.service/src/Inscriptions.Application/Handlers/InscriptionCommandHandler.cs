using Inscriptions.Application.Commands;
using Inscriptions.Domain.Ports.Input;
using MediatR;
using InscriptionEntity = Inscriptions.Domain.Entities.Inscription;

namespace Inscriptions.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les inscriptions.
/// </summary>
public sealed class InscriptionCommandHandler :
    IRequestHandler<CreateInscriptionCommand, InscriptionEntity>,
    IRequestHandler<UpdateInscriptionCommand, bool>,
    IRequestHandler<DeleteInscriptionCommand, bool>
{
    private readonly IInscriptionService _service;

    public InscriptionCommandHandler(IInscriptionService service)
    {
        _service = service;
    }

    public async Task<InscriptionEntity> Handle(CreateInscriptionCommand request, CancellationToken ct)
    {
        var entity = new InscriptionEntity
        {
            TypeId = request.TypeId,
            EleveId = request.EleveId,
            ClasseId = request.ClasseId,
            AnneeScolaireId = request.AnneeScolaireId,
            DateInscription = request.DateInscription,
            MontantInscription = request.MontantInscription,
            EstPaye = request.EstPaye,
            EstRedoublant = request.EstRedoublant,
            TypeEtablissement = request.TypeEtablissement,
            SerieBac = request.SerieBac,
            StatutInscription = request.StatutInscription,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateInscriptionCommand request, CancellationToken ct)
    {
        var entity = new InscriptionEntity
        {
            Id = request.Id,
            TypeId = request.TypeId,
            EleveId = request.EleveId,
            ClasseId = request.ClasseId,
            AnneeScolaireId = request.AnneeScolaireId,
            DateInscription = request.DateInscription,
            MontantInscription = request.MontantInscription,
            EstPaye = request.EstPaye,
            EstRedoublant = request.EstRedoublant,
            TypeEtablissement = request.TypeEtablissement,
            SerieBac = request.SerieBac,
            StatutInscription = request.StatutInscription,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteInscriptionCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
