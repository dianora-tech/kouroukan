using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Bde.Domain.Entities;
using Bde.Domain.Ports.Input;
using Bde.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Bde.Domain.Services;

/// <summary>
/// Service metier pour les membres BDE.
/// </summary>
public sealed class MembreBdeService : IMembreBdeService
{
    private const string Exchange = "kouroukan.events";

    private static readonly string[] RolesValides = ["President", "Tresorier", "Secretaire", "RespPole", "Membre"];

    private readonly IMembreBdeRepository _repository;
    private readonly IAssociationRepository _associationRepository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<MembreBdeService> _logger;

    public MembreBdeService(
        IMembreBdeRepository repository,
        IAssociationRepository associationRepository,
        IMessagePublisher publisher,
        ILogger<MembreBdeService> logger)
    {
        _repository = repository;
        _associationRepository = associationRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<MembreBde?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<MembreBde>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<MembreBde>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<MembreBde> CreateAsync(MembreBde entity, CancellationToken ct = default)
    {
        if (!RolesValides.Contains(entity.RoleBde))
            throw new InvalidOperationException(
                $"Role BDE invalide : '{entity.RoleBde}'. Valeurs acceptees : {string.Join(", ", RolesValides)}");

        if (!await _associationRepository.ExistsAsync(entity.AssociationId, ct).ConfigureAwait(false))
            throw new KeyNotFoundException($"L'association avec l'id {entity.AssociationId} n'existe pas.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Membre BDE {Id} cree pour l'eleve {EleveId} dans l'association {AssociationId}.",
            created.Id, created.EleveId, created.AssociationId);

        var evt = new EntityCreatedEvent<MembreBde>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.membrebde", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(MembreBde entity, CancellationToken ct = default)
    {
        if (!RolesValides.Contains(entity.RoleBde))
            throw new InvalidOperationException(
                $"Role BDE invalide : '{entity.RoleBde}'.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Membre BDE {Id} mis a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<MembreBde>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.membrebde", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Membre BDE {Id} supprime.", id);

            var evt = new EntityDeletedEvent<MembreBde>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.membrebde", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
