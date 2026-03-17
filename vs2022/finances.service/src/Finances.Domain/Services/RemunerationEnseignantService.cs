using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using Finances.Domain.Ports.Output;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Microsoft.Extensions.Logging;

namespace Finances.Domain.Services;

/// <summary>
/// Logique metier pour la gestion des remunerations des enseignants.
/// Supporte les modes Forfait, Heures et Mixte.
/// </summary>
public sealed class RemunerationEnseignantService : IRemunerationEnseignantService
{
    private const string Exchange = "kouroukan.events";
    private static readonly string[] ModesRemunerationAutorises = ["Forfait", "Heures", "Mixte"];
    private static readonly string[] StatutsPaiementAutorises = ["EnAttente", "Valide", "Paye"];

    private readonly IRemunerationEnseignantRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<RemunerationEnseignantService> _logger;

    public RemunerationEnseignantService(
        IRemunerationEnseignantRepository repository,
        IMessagePublisher publisher,
        ILogger<RemunerationEnseignantService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<RemunerationEnseignant?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RemunerationEnseignant>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PagedResult<RemunerationEnseignant>> GetPagedAsync(
        int page, int pageSize, string? search, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, orderBy, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<RemunerationEnseignant> CreateAsync(RemunerationEnseignant entity, CancellationToken ct = default)
    {
        var existing = await _repository.GetByEnseignantMoisAnneeAsync(
            entity.EnseignantId, entity.Mois, entity.Annee, ct).ConfigureAwait(false);
        if (existing is not null)
            throw new InvalidOperationException(
                $"Une remuneration existe deja pour l'enseignant {entity.EnseignantId} au mois {entity.Mois}/{entity.Annee}.");

        ValidateRemuneration(entity);
        CalculerMontantTotal(entity);

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation(
            "Remuneration creee pour l'enseignant {EnseignantId} — {Mois}/{Annee} — montant: {Montant} GNF (mode: {Mode}).",
            created.EnseignantId, created.Mois, created.Annee, created.MontantTotal, created.ModeRemuneration);

        var evt = new EntityCreatedEvent<RemunerationEnseignant>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.remunerationenseignant", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(RemunerationEnseignant entity, CancellationToken ct = default)
    {
        ValidateRemuneration(entity);
        CalculerMontantTotal(entity);

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Remuneration {Id} mise a jour — statut: {Statut}.",
                entity.Id, entity.StatutPaiement);

            var evt = new EntityUpdatedEvent<RemunerationEnseignant>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.remunerationenseignant", cancellationToken: ct)
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
            _logger.LogInformation("Remuneration {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<RemunerationEnseignant>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.remunerationenseignant", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    private static void ValidateRemuneration(RemunerationEnseignant entity)
    {
        if (!ModesRemunerationAutorises.Contains(entity.ModeRemuneration))
            throw new InvalidOperationException(
                $"Le mode de remuneration '{entity.ModeRemuneration}' n'est pas autorise. Valeurs acceptees : {string.Join(", ", ModesRemunerationAutorises)}");

        if (!StatutsPaiementAutorises.Contains(entity.StatutPaiement))
            throw new InvalidOperationException(
                $"Le statut de paiement '{entity.StatutPaiement}' n'est pas autorise. Valeurs acceptees : {string.Join(", ", StatutsPaiementAutorises)}");

        if (entity.Mois is < 1 or > 12)
            throw new InvalidOperationException("Le mois doit etre compris entre 1 et 12.");

        if (entity.Annee < 2000)
            throw new InvalidOperationException("L'annee doit etre superieure ou egale a 2000.");

        if (entity.ModeRemuneration is "Forfait" or "Mixte" && entity.MontantForfait is null or <= 0)
            throw new InvalidOperationException("Le montant forfait est obligatoire et doit etre positif pour le mode Forfait ou Mixte.");

        if (entity.ModeRemuneration is "Heures" or "Mixte")
        {
            if (entity.NombreHeures is null or <= 0)
                throw new InvalidOperationException("Le nombre d'heures est obligatoire et doit etre positif pour le mode Heures ou Mixte.");
            if (entity.TauxHoraire is null or <= 0)
                throw new InvalidOperationException("Le taux horaire est obligatoire et doit etre positif pour le mode Heures ou Mixte.");
        }
    }

    private static void CalculerMontantTotal(RemunerationEnseignant entity)
    {
        entity.MontantTotal = entity.ModeRemuneration switch
        {
            "Forfait" => entity.MontantForfait ?? 0,
            "Heures" => (entity.NombreHeures ?? 0) * (entity.TauxHoraire ?? 0),
            "Mixte" => (entity.MontantForfait ?? 0) + (entity.NombreHeures ?? 0) * (entity.TauxHoraire ?? 0),
            _ => entity.MontantTotal
        };
    }
}
