using Inscriptions.Application.Commands;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using MediatR;

namespace Inscriptions.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les eleves.
/// </summary>
public sealed class EleveCommandHandler :
    IRequestHandler<CreateEleveCommand, Eleve>,
    IRequestHandler<UpdateEleveCommand, bool>,
    IRequestHandler<DeleteEleveCommand, bool>
{
    private readonly IEleveService _service;

    public EleveCommandHandler(IEleveService service)
    {
        _service = service;
    }

    public async Task<Eleve> Handle(CreateEleveCommand request, CancellationToken ct)
    {
        var entity = new Eleve
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateNaissance = request.DateNaissance,
            LieuNaissance = request.LieuNaissance,
            Genre = request.Genre,
            Nationalite = request.Nationalite,
            Adresse = request.Adresse,
            PhotoUrl = request.PhotoUrl,
            NumeroMatricule = request.NumeroMatricule,
            NiveauClasseId = request.NiveauClasseId,
            ClasseId = request.ClasseId,
            ParentId = request.ParentId,
            StatutInscription = request.StatutInscription,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateEleveCommand request, CancellationToken ct)
    {
        var entity = new Eleve
        {
            Id = request.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateNaissance = request.DateNaissance,
            LieuNaissance = request.LieuNaissance,
            Genre = request.Genre,
            Nationalite = request.Nationalite,
            Adresse = request.Adresse,
            PhotoUrl = request.PhotoUrl,
            NumeroMatricule = request.NumeroMatricule,
            NiveauClasseId = request.NiveauClasseId,
            ClasseId = request.ClasseId,
            ParentId = request.ParentId,
            StatutInscription = request.StatutInscription,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteEleveCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
