using Inscriptions.Application.Commands;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using MediatR;

namespace Inscriptions.Application.Handlers;

/// <summary>
/// Gestionnaire des commandes pour les liaisons parent.
/// </summary>
public sealed class LiaisonParentCommandHandler :
    IRequestHandler<CreateLiaisonParentCommand, LiaisonParent>,
    IRequestHandler<DeleteLiaisonParentCommand, bool>
{
    private readonly ILiaisonParentService _service;

    public LiaisonParentCommandHandler(ILiaisonParentService service)
    {
        _service = service;
    }

    public async Task<LiaisonParent> Handle(CreateLiaisonParentCommand request, CancellationToken ct)
    {
        var entity = new LiaisonParent
        {
            ParentUserId = request.ParentUserId,
            EleveId = request.EleveId,
            CompanyId = request.CompanyId
        };
        return await _service.CreateAsync(entity, ct).ConfigureAwait(false);
    }

    public async Task<bool> Handle(DeleteLiaisonParentCommand request, CancellationToken ct)
    {
        return await _service.DeleteAsync(request.Id, ct).ConfigureAwait(false);
    }
}
