using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Pedagogie.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Pedagogie.Domain.Services;

/// <summary>
/// Service metier pour les salles.
/// </summary>
public sealed class SalleService : ISalleService
{
    private const string Exchange = "kouroukan.events";
    private readonly ISalleRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<SalleService> _logger;

    public SalleService(
        ISalleRepository repository,
        IMessagePublisher publisher,
        ILogger<SalleService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Salle?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Salle>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Salle>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Salle> CreateAsync(Salle entity, CancellationToken ct = default)
    {
        if (entity.Capacite <= 0)
            throw new InvalidOperationException("La capacite de la salle doit etre superieure a 0.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Salle {Name} creee avec l'id {Id}.", created.Name, created.Id);

        var evt = new EntityCreatedEvent<Salle>(created) { EntityId = created.Id };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.salle", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Salle entity, CancellationToken ct = default)
    {
        if (entity.Capacite <= 0)
            throw new InvalidOperationException("La capacite de la salle doit etre superieure a 0.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Salle {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Salle>(entity) { EntityId = entity.Id };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.salle", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Salle {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<Salle>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.salle", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
