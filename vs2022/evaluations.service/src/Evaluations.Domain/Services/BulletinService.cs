using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Input;
using Evaluations.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Evaluations.Domain.Services;

/// <summary>
/// Service metier pour les bulletins de notes.
/// </summary>
public sealed class BulletinService : IBulletinService
{
    private const string Exchange = "kouroukan.events";

    private readonly IBulletinRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<BulletinService> _logger;

    public BulletinService(
        IBulletinRepository repository,
        IMessagePublisher publisher,
        ILogger<BulletinService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Bulletin?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Bulletin>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Bulletin>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Bulletin> CreateAsync(Bulletin entity, CancellationToken ct = default)
    {
        if (entity.Trimestre is < 1 or > 3)
            throw new InvalidOperationException(
                $"Le trimestre doit etre compris entre 1 et 3. Valeur recue : {entity.Trimestre}.");

        if (entity.MoyenneGenerale < 0 || entity.MoyenneGenerale > 20)
            throw new InvalidOperationException("La moyenne generale doit etre comprise entre 0 et 20.");

        var existing = await _repository.GetByEleveTrimestreAsync(
            entity.EleveId, entity.Trimestre, entity.AnneeScolaireId, ct).ConfigureAwait(false);
        if (existing is not null)
            throw new InvalidOperationException(
                $"Un bulletin existe deja pour l'eleve {entity.EleveId} au trimestre {entity.Trimestre} de l'annee scolaire {entity.AnneeScolaireId}.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Bulletin {Id} cree pour l'eleve {EleveId} au trimestre {Trimestre}.",
            created.Id, created.EleveId, created.Trimestre);

        var evt = new EntityCreatedEvent<Bulletin>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.bulletin", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Bulletin entity, CancellationToken ct = default)
    {
        if (entity.Trimestre is < 1 or > 3)
            throw new InvalidOperationException(
                $"Le trimestre doit etre compris entre 1 et 3. Valeur recue : {entity.Trimestre}.");

        if (entity.MoyenneGenerale < 0 || entity.MoyenneGenerale > 20)
            throw new InvalidOperationException("La moyenne generale doit etre comprise entre 0 et 20.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Bulletin {Id} mis a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Bulletin>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.bulletin", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Bulletin {Id} supprime.", id);

            var evt = new EntityDeletedEvent<Bulletin>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.bulletin", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
