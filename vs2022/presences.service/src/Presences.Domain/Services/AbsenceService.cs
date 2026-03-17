using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Presences.Domain.Entities;
using Presences.Domain.Ports.Input;
using Presences.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Presences.Domain.Services;

/// <summary>
/// Service metier pour les absences.
/// </summary>
public sealed class AbsenceService : IAbsenceService
{
    private const string Exchange = "kouroukan.events";

    private readonly IAbsenceRepository _repository;
    private readonly IAppelRepository _appelRepository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<AbsenceService> _logger;

    public AbsenceService(
        IAbsenceRepository repository,
        IAppelRepository appelRepository,
        IMessagePublisher publisher,
        ILogger<AbsenceService> logger)
    {
        _repository = repository;
        _appelRepository = appelRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Absence?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Absence>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Absence>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Absence> CreateAsync(Absence entity, CancellationToken ct = default)
    {
        if (entity.AppelId.HasValue &&
            !await _appelRepository.ExistsAsync(entity.AppelId.Value, ct).ConfigureAwait(false))
            throw new KeyNotFoundException($"L'appel avec l'id {entity.AppelId} n'existe pas.");

        if (entity.EstJustifiee && string.IsNullOrWhiteSpace(entity.MotifJustification))
            throw new InvalidOperationException("Le motif de justification est obligatoire pour une absence justifiee.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Absence {Id} creee pour l'eleve {EleveId} le {DateAbsence}.",
            created.Id, created.EleveId, created.DateAbsence);

        var evt = new EntityCreatedEvent<Absence>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.absence", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Absence entity, CancellationToken ct = default)
    {
        if (entity.EstJustifiee && string.IsNullOrWhiteSpace(entity.MotifJustification))
            throw new InvalidOperationException("Le motif de justification est obligatoire pour une absence justifiee.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Absence {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Absence>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.absence", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Absence {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<Absence>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.absence", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
