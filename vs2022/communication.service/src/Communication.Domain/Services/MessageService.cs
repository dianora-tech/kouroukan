using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Input;
using Communication.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Communication.Domain.Services;

/// <summary>
/// Service metier pour les messages.
/// </summary>
public sealed class MessageService : IMessageService
{
    private const string Exchange = "kouroukan.events";

    private readonly IMessageRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<MessageService> _logger;

    public MessageService(
        IMessageRepository repository,
        IMessagePublisher publisher,
        ILogger<MessageService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Message?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Message>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Message>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Message> CreateAsync(Message entity, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Sujet))
            throw new InvalidOperationException("Le sujet du message est obligatoire.");

        if (string.IsNullOrWhiteSpace(entity.Contenu))
            throw new InvalidOperationException("Le contenu du message est obligatoire.");

        if (entity.DestinataireId is null && string.IsNullOrWhiteSpace(entity.GroupeDestinataire))
            throw new InvalidOperationException(
                "Un message doit avoir soit un destinataire individuel, soit un groupe destinataire.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Message {Id} cree par l'expediteur {ExpediteurId}.",
            created.Id, created.ExpediteurId);

        var evt = new EntityCreatedEvent<Message>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.message", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Message entity, CancellationToken ct = default)
    {
        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Message {Id} mis a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Message>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.message", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Message {Id} supprime.", id);

            var evt = new EntityDeletedEvent<Message>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.message", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
