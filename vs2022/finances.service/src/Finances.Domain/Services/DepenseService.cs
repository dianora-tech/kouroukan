using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using Finances.Domain.Ports.Output;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Microsoft.Extensions.Logging;

namespace Finances.Domain.Services;

/// <summary>
/// Logique metier pour la gestion des depenses avec workflow de validation multi-niveaux.
/// </summary>
public sealed class DepenseService : IDepenseService
{
    private const string Exchange = "kouroukan.events";
    private static readonly string[] CategoriesAutorisees =
        ["Personnel", "Fournitures", "Maintenance", "Evenements", "BDE", "Equipements"];
    private static readonly string[] StatutsAutorisees =
        ["Demande", "ValideN1", "ValideFinance", "ValideDirection", "Executee", "Archivee"];

    private readonly IDepenseRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<DepenseService> _logger;

    public DepenseService(
        IDepenseRepository repository,
        IMessagePublisher publisher,
        ILogger<DepenseService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Depense?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Depense>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PagedResult<Depense>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Depense> CreateAsync(Depense entity, CancellationToken ct = default)
    {
        var existing = await _repository.GetByNumeroJustificatifAsync(entity.NumeroJustificatif, ct).ConfigureAwait(false);
        if (existing is not null)
            throw new InvalidOperationException($"Une depense avec le numero de justificatif '{entity.NumeroJustificatif}' existe deja.");

        if (entity.Montant <= 0)
            throw new InvalidOperationException("Le montant de la depense doit etre superieur a zero.");

        if (!CategoriesAutorisees.Contains(entity.Categorie))
            throw new InvalidOperationException(
                $"La categorie '{entity.Categorie}' n'est pas autorisee. Valeurs acceptees : {string.Join(", ", CategoriesAutorisees)}");

        if (!StatutsAutorisees.Contains(entity.StatutDepense))
            throw new InvalidOperationException(
                $"Le statut '{entity.StatutDepense}' n'est pas autorise. Valeurs acceptees : {string.Join(", ", StatutsAutorisees)}");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Depense {NumeroJustificatif} creee (montant: {Montant} GNF, categorie: {Categorie}).",
            created.NumeroJustificatif, created.Montant, created.Categorie);

        var evt = new EntityCreatedEvent<Depense>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.depense", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Depense entity, CancellationToken ct = default)
    {
        if (entity.Montant <= 0)
            throw new InvalidOperationException("Le montant de la depense doit etre superieur a zero.");

        if (!CategoriesAutorisees.Contains(entity.Categorie))
            throw new InvalidOperationException(
                $"La categorie '{entity.Categorie}' n'est pas autorisee.");

        if (!StatutsAutorisees.Contains(entity.StatutDepense))
            throw new InvalidOperationException(
                $"Le statut '{entity.StatutDepense}' n'est pas autorise.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Depense {Id} ({NumeroJustificatif}) mise a jour — statut: {Statut}.",
                entity.Id, entity.NumeroJustificatif, entity.StatutDepense);

            var evt = new EntityUpdatedEvent<Depense>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.depense", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Depense {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<Depense>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.depense", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
