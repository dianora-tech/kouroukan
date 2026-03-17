using Documents.Application.Commands;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Input;
using MediatR;

namespace Documents.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les modeles de documents.
/// </summary>
public sealed class ModeleDocumentCommandHandler :
    IRequestHandler<CreateModeleDocumentCommand, ModeleDocument>,
    IRequestHandler<UpdateModeleDocumentCommand, bool>,
    IRequestHandler<DeleteModeleDocumentCommand, bool>
{
    private readonly IModeleDocumentService _service;

    public ModeleDocumentCommandHandler(IModeleDocumentService service)
    {
        _service = service;
    }

    public async Task<ModeleDocument> Handle(CreateModeleDocumentCommand request, CancellationToken ct)
    {
        var entity = new ModeleDocument
        {
            TypeId = request.TypeId,
            Code = request.Code,
            Contenu = request.Contenu,
            LogoUrl = request.LogoUrl,
            CouleurPrimaire = request.CouleurPrimaire,
            TextePiedPage = request.TextePiedPage,
            EstActif = request.EstActif,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(UpdateModeleDocumentCommand request, CancellationToken ct)
    {
        var entity = new ModeleDocument
        {
            Id = request.Id,
            TypeId = request.TypeId,
            Code = request.Code,
            Contenu = request.Contenu,
            LogoUrl = request.LogoUrl,
            CouleurPrimaire = request.CouleurPrimaire,
            TextePiedPage = request.TextePiedPage,
            EstActif = request.EstActif,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteModeleDocumentCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
