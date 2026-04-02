using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Inscriptions.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Inscriptions.Domain.Services;

/// <summary>
/// Service metier pour les radiations d'eleves.
/// </summary>
public sealed class RadiationService : IRadiationService
{
    private const string Exchange = "kouroukan.events";
    private readonly IRadiationRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<RadiationService> _logger;

    public RadiationService(
        IRadiationRepository repository,
        IMessagePublisher publisher,
        ILogger<RadiationService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Radiation?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Radiation>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Radiation>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Radiation> CreateAsync(Radiation entity, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Motif))
            throw new InvalidOperationException("Le motif de la radiation est obligatoire.");

        entity.DateRadiation = DateTime.UtcNow;

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Radiation creee avec l'id {Id} pour l'eleve {EleveId}.", created.Id, created.EleveId);

        var evt = new EntityCreatedEvent<Radiation>(created) { EntityId = created.Id };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.radiation", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }
}
