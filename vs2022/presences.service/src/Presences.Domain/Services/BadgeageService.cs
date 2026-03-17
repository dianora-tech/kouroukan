using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Presences.Domain.Entities;
using Presences.Domain.Ports.Input;
using Presences.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Presences.Domain.Services;

/// <summary>
/// Service metier pour les badgeages.
/// </summary>
public sealed class BadgeageService : IBadgeageService
{
    private const string Exchange = "kouroukan.events";

    private static readonly string[] PointsAccesValides = ["Entree", "Sortie", "Cantine", "Biblio"];
    private static readonly string[] MethodesBadgeageValides = ["NFC", "QRCode", "Manuel"];

    private readonly IBadgeageRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<BadgeageService> _logger;

    public BadgeageService(
        IBadgeageRepository repository,
        IMessagePublisher publisher,
        ILogger<BadgeageService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Badgeage?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Badgeage>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Badgeage>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Badgeage> CreateAsync(Badgeage entity, CancellationToken ct = default)
    {
        if (!PointsAccesValides.Contains(entity.PointAcces))
            throw new InvalidOperationException(
                $"Point d'acces invalide : '{entity.PointAcces}'. Valeurs acceptees : {string.Join(", ", PointsAccesValides)}");

        if (!MethodesBadgeageValides.Contains(entity.MethodeBadgeage))
            throw new InvalidOperationException(
                $"Methode de badgeage invalide : '{entity.MethodeBadgeage}'. Valeurs acceptees : {string.Join(", ", MethodesBadgeageValides)}");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Badgeage {Id} cree pour l'eleve {EleveId} au point {PointAcces}.",
            created.Id, created.EleveId, created.PointAcces);

        var evt = new EntityCreatedEvent<Badgeage>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.badgeage", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Badgeage entity, CancellationToken ct = default)
    {
        if (!PointsAccesValides.Contains(entity.PointAcces))
            throw new InvalidOperationException(
                $"Point d'acces invalide : '{entity.PointAcces}'.");

        if (!MethodesBadgeageValides.Contains(entity.MethodeBadgeage))
            throw new InvalidOperationException(
                $"Methode de badgeage invalide : '{entity.MethodeBadgeage}'.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Badgeage {Id} mis a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Badgeage>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.badgeage", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Badgeage {Id} supprime.", id);

            var evt = new EntityDeletedEvent<Badgeage>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.badgeage", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
