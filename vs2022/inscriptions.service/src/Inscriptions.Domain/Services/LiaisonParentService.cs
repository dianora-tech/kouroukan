using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Inscriptions.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Inscriptions.Domain.Services;

/// <summary>
/// Service metier pour les liaisons parent-eleve.
/// </summary>
public sealed class LiaisonParentService : ILiaisonParentService
{
    private const string Exchange = "kouroukan.events";
    private readonly ILiaisonParentRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<LiaisonParentService> _logger;

    public LiaisonParentService(
        ILiaisonParentRepository repository,
        IMessagePublisher publisher,
        ILogger<LiaisonParentService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<LiaisonParent?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<LiaisonParent>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<LiaisonParent>> GetPagedAsync(
        int page, int pageSize, int? parentUserId, int? companyId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, parentUserId, companyId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<LiaisonParent> CreateAsync(LiaisonParent entity, CancellationToken ct = default)
    {
        entity.Statut = "Active";

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("LiaisonParent creee avec l'id {Id} pour le parent {ParentUserId} et l'eleve {EleveId}.",
            created.Id, created.ParentUserId, created.EleveId);

        var evt = new EntityCreatedEvent<LiaisonParent>(created) { EntityId = created.Id };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.liaison-parent", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("LiaisonParent {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<LiaisonParent>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.liaison-parent", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
