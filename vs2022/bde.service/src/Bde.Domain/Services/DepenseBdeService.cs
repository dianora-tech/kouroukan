using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Bde.Domain.Entities;
using Bde.Domain.Ports.Input;
using Bde.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Bde.Domain.Services;

/// <summary>
/// Service metier pour les depenses BDE.
/// </summary>
public sealed class DepenseBdeService : IDepenseBdeService
{
    private const string Exchange = "kouroukan.events";

    private static readonly string[] StatutsValides = ["Demandee", "ValideTresorier", "ValideSuper", "Refusee"];
    private static readonly string[] CategoriesValides = ["Materiel", "Location", "Prestataire", "Remboursement"];

    private readonly IDepenseBdeRepository _repository;
    private readonly IAssociationRepository _associationRepository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<DepenseBdeService> _logger;

    public DepenseBdeService(
        IDepenseBdeRepository repository,
        IAssociationRepository associationRepository,
        IMessagePublisher publisher,
        ILogger<DepenseBdeService> logger)
    {
        _repository = repository;
        _associationRepository = associationRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<DepenseBde?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<DepenseBde>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<DepenseBde>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<DepenseBde> CreateAsync(DepenseBde entity, CancellationToken ct = default)
    {
        if (!StatutsValides.Contains(entity.StatutValidation))
            throw new InvalidOperationException(
                $"Statut de validation invalide : '{entity.StatutValidation}'.");

        if (!CategoriesValides.Contains(entity.Categorie))
            throw new InvalidOperationException(
                $"Categorie invalide : '{entity.Categorie}'.");

        if (entity.Montant <= 0)
            throw new InvalidOperationException("Le montant de la depense doit etre superieur a zero.");

        if (!await _associationRepository.ExistsAsync(entity.AssociationId, ct).ConfigureAwait(false))
            throw new KeyNotFoundException($"L'association avec l'id {entity.AssociationId} n'existe pas.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Depense BDE {Id} creee pour l'association {AssociationId}, montant {Montant} GNF.",
            created.Id, created.AssociationId, created.Montant);

        var evt = new EntityCreatedEvent<DepenseBde>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.depensebde", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(DepenseBde entity, CancellationToken ct = default)
    {
        if (!StatutsValides.Contains(entity.StatutValidation))
            throw new InvalidOperationException(
                $"Statut de validation invalide : '{entity.StatutValidation}'.");

        if (!CategoriesValides.Contains(entity.Categorie))
            throw new InvalidOperationException(
                $"Categorie invalide : '{entity.Categorie}'.");

        if (entity.Montant <= 0)
            throw new InvalidOperationException("Le montant de la depense doit etre superieur a zero.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Depense BDE {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<DepenseBde>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.depensebde", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Depense BDE {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<DepenseBde>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.depensebde", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
