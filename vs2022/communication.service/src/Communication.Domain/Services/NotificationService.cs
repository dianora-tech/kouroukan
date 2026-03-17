using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Input;
using Communication.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Communication.Domain.Services;

/// <summary>
/// Service metier pour les notifications.
/// </summary>
public sealed class NotificationService : INotificationService
{
    private const string Exchange = "kouroukan.events";

    private readonly INotificationRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository repository,
        IMessagePublisher publisher,
        ILogger<NotificationService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Notification?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Notification>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Notification>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Notification> CreateAsync(Notification entity, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Contenu))
            throw new InvalidOperationException("Le contenu de la notification est obligatoire.");

        if (string.IsNullOrWhiteSpace(entity.Canal))
            throw new InvalidOperationException("Le canal de la notification est obligatoire.");

        var allowedCanals = new[] { "Push", "SMS", "Email", "InApp" };
        if (!allowedCanals.Contains(entity.Canal))
            throw new InvalidOperationException(
                $"Le canal '{entity.Canal}' n'est pas valide. Canaux autorises : {string.Join(", ", allowedCanals)}.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Notification {Id} creee sur le canal {Canal}.",
            created.Id, created.Canal);

        var evt = new EntityCreatedEvent<Notification>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.notification", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Notification entity, CancellationToken ct = default)
    {
        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Notification {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Notification>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.notification", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Notification {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<Notification>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.notification", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
