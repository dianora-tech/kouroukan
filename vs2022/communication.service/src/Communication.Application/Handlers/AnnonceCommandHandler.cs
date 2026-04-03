using Communication.Application.Commands;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Input;
using MediatR;

namespace Communication.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les annonces.
/// </summary>
public sealed class AnnonceCommandHandler :
    IRequestHandler<CreateAnnonceCommand, Annonce>,
    IRequestHandler<UpdateAnnonceCommand, bool>,
    IRequestHandler<DeleteAnnonceCommand, bool>
{
    private readonly IAnnonceService _service;

    public AnnonceCommandHandler(IAnnonceService service)
    {
        _service = service;
    }

    public async Task<Annonce> Handle(CreateAnnonceCommand request, CancellationToken ct)
    {
        var entity = new Annonce
        {
            Name = request.Name,
            TypeId = request.TypeId,
            Contenu = request.Contenu,
            DateDebut = request.DateDebut,
            DateFin = request.DateFin,
            EstActive = request.EstActive,
            CibleAudience = request.CibleAudience,
            Priorite = request.Priorite,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateAnnonceCommand request, CancellationToken ct)
    {
        var entity = new Annonce
        {
            Id = request.Id,
            Name = request.Name,
            TypeId = request.TypeId,
            Contenu = request.Contenu,
            DateDebut = request.DateDebut,
            DateFin = request.DateFin,
            EstActive = request.EstActive,
            CibleAudience = request.CibleAudience,
            Priorite = request.Priorite,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteAnnonceCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
