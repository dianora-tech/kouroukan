using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using Finances.Domain.Ports.Output;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Microsoft.Extensions.Logging;

namespace Finances.Domain.Services;

/// <summary>
/// Logique metier pour la gestion des paiements (Mobile Money + especes).
/// </summary>
public sealed class PaiementService : IPaiementService
{
    private const string Exchange = "kouroukan.events";
    private static readonly string[] MoyensPaiementAutorises =
        ["OrangeMoney", "SoutraMoney", "MTNMoMo", "Especes"];

    private readonly IPaiementRepository _repository;
    private readonly IFactureRepository _factureRepository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<PaiementService> _logger;

    public PaiementService(
        IPaiementRepository repository,
        IFactureRepository factureRepository,
        IMessagePublisher publisher,
        ILogger<PaiementService> logger)
    {
        _repository = repository;
        _factureRepository = factureRepository;
        _publisher = publisher;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Paiement?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Paiement>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PagedResult<Paiement>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Paiement> CreateAsync(Paiement entity, CancellationToken ct = default)
    {
        var existingRecu = await _repository.GetByNumeroRecuAsync(entity.NumeroRecu, ct).ConfigureAwait(false);
        if (existingRecu is not null)
            throw new InvalidOperationException($"Un paiement avec le numero de recu '{entity.NumeroRecu}' existe deja.");

        if (!MoyensPaiementAutorises.Contains(entity.MoyenPaiement))
            throw new InvalidOperationException(
                $"Le moyen de paiement '{entity.MoyenPaiement}' n'est pas autorise. Valeurs acceptees : {string.Join(", ", MoyensPaiementAutorises)}");

        if (entity.MontantPaye <= 0)
            throw new InvalidOperationException("Le montant paye doit etre superieur a zero.");

        var facture = await _factureRepository.GetByIdAsync(entity.FactureId, ct).ConfigureAwait(false);
        if (facture is null)
            throw new KeyNotFoundException($"La facture {entity.FactureId} est introuvable.");

        if (entity.MoyenPaiement != "Especes" && string.IsNullOrWhiteSpace(entity.ReferenceMobileMoney))
            throw new InvalidOperationException("La reference Mobile Money est obligatoire pour les paiements electroniques.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Paiement {NumeroRecu} cree (montant: {Montant} GNF, moyen: {Moyen}) sur facture {FactureId}.",
            created.NumeroRecu, created.MontantPaye, created.MoyenPaiement, created.FactureId);

        var evt = new EntityCreatedEvent<Paiement>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.paiement", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Paiement entity, CancellationToken ct = default)
    {
        if (!MoyensPaiementAutorises.Contains(entity.MoyenPaiement))
            throw new InvalidOperationException(
                $"Le moyen de paiement '{entity.MoyenPaiement}' n'est pas autorise.");

        if (entity.MontantPaye <= 0)
            throw new InvalidOperationException("Le montant paye doit etre superieur a zero.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Paiement {Id} ({NumeroRecu}) mis a jour.", entity.Id, entity.NumeroRecu);

            var evt = new EntityUpdatedEvent<Paiement>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.paiement", cancellationToken: ct)
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
            _logger.LogInformation("Paiement {Id} supprime.", id);

            var evt = new EntityDeletedEvent<Paiement>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.paiement", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
