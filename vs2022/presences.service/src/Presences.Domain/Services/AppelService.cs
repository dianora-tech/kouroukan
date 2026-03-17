using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Presences.Domain.Entities;
using Presences.Domain.Ports.Input;
using Presences.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Presences.Domain.Services;

/// <summary>
/// Service metier pour les appels.
/// </summary>
public sealed class AppelService : IAppelService
{
    private const string Exchange = "kouroukan.events";

    private readonly IAppelRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<AppelService> _logger;

    public AppelService(
        IAppelRepository repository,
        IMessagePublisher publisher,
        ILogger<AppelService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Appel?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Appel>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Appel>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Appel> CreateAsync(Appel entity, CancellationToken ct = default)
    {
        if (entity.HeureAppel == default)
            throw new InvalidOperationException("L'heure de l'appel est obligatoire.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Appel {Id} cree pour la classe {ClasseId} par l'enseignant {EnseignantId}.",
            created.Id, created.ClasseId, created.EnseignantId);

        var evt = new EntityCreatedEvent<Appel>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.appel", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Appel entity, CancellationToken ct = default)
    {
        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Appel {Id} mis a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Appel>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.appel", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Appel {Id} supprime.", id);

            var evt = new EntityDeletedEvent<Appel>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.appel", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
