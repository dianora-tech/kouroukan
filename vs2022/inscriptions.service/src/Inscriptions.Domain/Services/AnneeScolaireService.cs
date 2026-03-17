using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Inscriptions.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Inscriptions.Domain.Services;

/// <summary>
/// Service metier pour les annees scolaires.
/// </summary>
public sealed class AnneeScolaireService : IAnneeScolaireService
{
    private const string Exchange = "kouroukan.events";
    private readonly IAnneeScolaireRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<AnneeScolaireService> _logger;

    public AnneeScolaireService(
        IAnneeScolaireRepository repository,
        IMessagePublisher publisher,
        ILogger<AnneeScolaireService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<AnneeScolaire?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<AnneeScolaire>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<AnneeScolaire>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<AnneeScolaire> CreateAsync(AnneeScolaire entity, CancellationToken ct = default)
    {
        if (entity.DateFin <= entity.DateDebut)
            throw new InvalidOperationException("La date de fin doit etre posterieure a la date de debut.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Annee scolaire {Libelle} creee avec l'id {Id}.", created.Libelle, created.Id);

        var evt = new EntityCreatedEvent<AnneeScolaire>(created) { EntityId = created.Id };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.anneescolaire", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(AnneeScolaire entity, CancellationToken ct = default)
    {
        if (entity.DateFin <= entity.DateDebut)
            throw new InvalidOperationException("La date de fin doit etre posterieure a la date de debut.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Annee scolaire {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<AnneeScolaire>(entity) { EntityId = entity.Id };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.anneescolaire", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Annee scolaire {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<AnneeScolaire>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.anneescolaire", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
