using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Communication.Domain.Entities;
using Communication.Domain.Ports.Input;
using Communication.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Communication.Domain.Services;

/// <summary>
/// Service metier pour les annonces.
/// </summary>
public sealed class AnnonceService : IAnnonceService
{
    private const string Exchange = "kouroukan.events";

    private readonly IAnnonceRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<AnnonceService> _logger;

    public AnnonceService(
        IAnnonceRepository repository,
        IMessagePublisher publisher,
        ILogger<AnnonceService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Annonce?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Annonce>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Annonce>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Annonce> CreateAsync(Annonce entity, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Contenu))
            throw new InvalidOperationException("Le contenu de l'annonce est obligatoire.");

        if (entity.DateFin.HasValue && entity.DateFin < entity.DateDebut)
            throw new InvalidOperationException("La date de fin doit etre posterieure a la date de debut.");

        if (entity.Priorite < 1)
            throw new InvalidOperationException("La priorite doit etre superieure ou egale a 1.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Annonce {Id} creee pour l'audience {CibleAudience}.",
            created.Id, created.CibleAudience);

        var evt = new EntityCreatedEvent<Annonce>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.annonce", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Annonce entity, CancellationToken ct = default)
    {
        if (entity.DateFin.HasValue && entity.DateFin < entity.DateDebut)
            throw new InvalidOperationException("La date de fin doit etre posterieure a la date de debut.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Annonce {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Annonce>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.annonce", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Annonce {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<Annonce>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.annonce", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
