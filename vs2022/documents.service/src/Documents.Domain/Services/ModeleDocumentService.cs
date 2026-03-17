using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Input;
using Documents.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Documents.Domain.Services;

/// <summary>
/// Service metier pour les modeles de documents.
/// </summary>
public sealed class ModeleDocumentService : IModeleDocumentService
{
    private const string Exchange = "kouroukan.events";

    private readonly IModeleDocumentRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<ModeleDocumentService> _logger;

    public ModeleDocumentService(
        IModeleDocumentRepository repository,
        IMessagePublisher publisher,
        ILogger<ModeleDocumentService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<ModeleDocument?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<ModeleDocument>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<ModeleDocument>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<ModeleDocument> CreateAsync(ModeleDocument entity, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Code))
            throw new InvalidOperationException("Le code du modele de document est obligatoire.");

        if (string.IsNullOrWhiteSpace(entity.Contenu))
            throw new InvalidOperationException("Le contenu du modele de document est obligatoire.");

        if (entity.CouleurPrimaire is not null && !entity.CouleurPrimaire.StartsWith('#'))
            throw new InvalidOperationException("La couleur primaire doit etre au format hexadecimal (ex: #16a34a).");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Modele de document {Code} cree avec l'id {Id}.",
            created.Code, created.Id);

        var evt = new EntityCreatedEvent<ModeleDocument>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.modeledocument", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(ModeleDocument entity, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Code))
            throw new InvalidOperationException("Le code du modele de document est obligatoire.");

        if (string.IsNullOrWhiteSpace(entity.Contenu))
            throw new InvalidOperationException("Le contenu du modele de document est obligatoire.");

        if (entity.CouleurPrimaire is not null && !entity.CouleurPrimaire.StartsWith('#'))
            throw new InvalidOperationException("La couleur primaire doit etre au format hexadecimal (ex: #16a34a).");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Modele de document {Id} mis a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<ModeleDocument>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.modeledocument", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Modele de document {Id} supprime.", id);

            var evt = new EntityDeletedEvent<ModeleDocument>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.modeledocument", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
