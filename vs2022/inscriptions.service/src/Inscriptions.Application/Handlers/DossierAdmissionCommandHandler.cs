using Inscriptions.Application.Commands;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using MediatR;

namespace Inscriptions.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les dossiers d'admission.
/// </summary>
public sealed class DossierAdmissionCommandHandler :
    IRequestHandler<CreateDossierAdmissionCommand, DossierAdmission>,
    IRequestHandler<UpdateDossierAdmissionCommand, bool>,
    IRequestHandler<DeleteDossierAdmissionCommand, bool>
{
    private readonly IDossierAdmissionService _service;

    public DossierAdmissionCommandHandler(IDossierAdmissionService service)
    {
        _service = service;
    }

    public async Task<DossierAdmission> Handle(CreateDossierAdmissionCommand request, CancellationToken ct)
    {
        var entity = new DossierAdmission
        {
            TypeId = request.TypeId,
            EleveId = request.EleveId,
            AnneeScolaireId = request.AnneeScolaireId,
            StatutDossier = request.StatutDossier,
            EtapeActuelle = request.EtapeActuelle,
            DateDemande = request.DateDemande,
            DateDecision = request.DateDecision,
            MotifRefus = request.MotifRefus,
            ScoringInterne = request.ScoringInterne,
            Commentaires = request.Commentaires,
            ResponsableAdmissionId = request.ResponsableAdmissionId,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateDossierAdmissionCommand request, CancellationToken ct)
    {
        var entity = new DossierAdmission
        {
            Id = request.Id,
            TypeId = request.TypeId,
            EleveId = request.EleveId,
            AnneeScolaireId = request.AnneeScolaireId,
            StatutDossier = request.StatutDossier,
            EtapeActuelle = request.EtapeActuelle,
            DateDemande = request.DateDemande,
            DateDecision = request.DateDecision,
            MotifRefus = request.MotifRefus,
            ScoringInterne = request.ScoringInterne,
            Commentaires = request.Commentaires,
            ResponsableAdmissionId = request.ResponsableAdmissionId,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteDossierAdmissionCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
