using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Pedagogie.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Pedagogie.Domain.Services;

/// <summary>
/// Service metier pour les cahiers de textes.
/// </summary>
public sealed class CahierTextesService : ICahierTextesService
{
    private const string Exchange = "kouroukan.events";
    private readonly ICahierTextesRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<CahierTextesService> _logger;

    public CahierTextesService(
        ICahierTextesRepository repository,
        IMessagePublisher publisher,
        ILogger<CahierTextesService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<CahierTextes?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<CahierTextes>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<CahierTextes>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<CahierTextes> CreateAsync(CahierTextes entity, CancellationToken ct = default)
    {
        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Cahier de textes cree avec l'id {Id} pour la seance {SeanceId}.", created.Id, created.SeanceId);

        var evt = new EntityCreatedEvent<CahierTextes>(created) { EntityId = created.Id };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.cahiertextes", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(CahierTextes entity, CancellationToken ct = default)
    {
        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Cahier de textes {Id} mis a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<CahierTextes>(entity) { EntityId = entity.Id };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.cahiertextes", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Cahier de textes {Id} supprime.", id);

            var evt = new EntityDeletedEvent<CahierTextes>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.cahiertextes", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
