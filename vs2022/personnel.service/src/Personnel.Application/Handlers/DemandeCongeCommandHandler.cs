using Personnel.Application.Commands;
using Personnel.Domain.Entities;
using Personnel.Domain.Ports.Input;
using MediatR;

namespace Personnel.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les demandes de conge.
/// </summary>
public sealed class DemandeCongeCommandHandler :
    IRequestHandler<CreateDemandeCongeCommand, DemandeConge>,
    IRequestHandler<UpdateDemandeCongeCommand, bool>,
    IRequestHandler<DeleteDemandeCongeCommand, bool>
{
    private readonly IDemandeCongeService _service;

    public DemandeCongeCommandHandler(IDemandeCongeService service)
    {
        _service = service;
    }

    public async Task<DemandeConge> Handle(CreateDemandeCongeCommand request, CancellationToken ct)
    {
        var entity = new DemandeConge
        {
            Name = request.Name,
            Description = request.Description,
            EnseignantId = request.EnseignantId,
            DateDebut = request.DateDebut,
            DateFin = request.DateFin,
            Motif = request.Motif,
            StatutDemande = request.StatutDemande,
            PieceJointeUrl = request.PieceJointeUrl,
            CommentaireValidateur = request.CommentaireValidateur,
            ValidateurId = request.ValidateurId,
            DateValidation = request.DateValidation,
            ImpactPaie = request.ImpactPaie,
            TypeId = request.TypeId,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateDemandeCongeCommand request, CancellationToken ct)
    {
        var entity = new DemandeConge
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            EnseignantId = request.EnseignantId,
            DateDebut = request.DateDebut,
            DateFin = request.DateFin,
            Motif = request.Motif,
            StatutDemande = request.StatutDemande,
            PieceJointeUrl = request.PieceJointeUrl,
            CommentaireValidateur = request.CommentaireValidateur,
            ValidateurId = request.ValidateurId,
            DateValidation = request.DateValidation,
            ImpactPaie = request.ImpactPaie,
            TypeId = request.TypeId,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteDemandeCongeCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
