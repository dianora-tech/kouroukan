using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Inscriptions.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Inscriptions.Domain.Services;

/// <summary>
/// Service metier pour les dossiers d'admission.
/// </summary>
public sealed class DossierAdmissionService : IDossierAdmissionService
{
    private const string Exchange = "kouroukan.events";

    private static readonly string[] StatutsDossierValides =
        ["Prospect", "PreInscrit", "EnEtude", "Convoque", "Admis", "Refuse", "ListeAttente"];

    private readonly IDossierAdmissionRepository _repository;
    private readonly IEleveRepository _eleveRepository;
    private readonly IAnneeScolaireRepository _anneeScolaireRepository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<DossierAdmissionService> _logger;

    public DossierAdmissionService(
        IDossierAdmissionRepository repository,
        IEleveRepository eleveRepository,
        IAnneeScolaireRepository anneeScolaireRepository,
        IMessagePublisher publisher,
        ILogger<DossierAdmissionService> logger)
    {
        _repository = repository;
        _eleveRepository = eleveRepository;
        _anneeScolaireRepository = anneeScolaireRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<DossierAdmission?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<DossierAdmission>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<DossierAdmission>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<DossierAdmission> CreateAsync(DossierAdmission entity, CancellationToken ct = default)
    {
        if (!StatutsDossierValides.Contains(entity.StatutDossier))
            throw new InvalidOperationException(
                $"Statut de dossier invalide : '{entity.StatutDossier}'. Valeurs acceptees : {string.Join(", ", StatutsDossierValides)}");

        if (!await _eleveRepository.ExistsAsync(entity.EleveId, ct).ConfigureAwait(false))
            throw new KeyNotFoundException($"L'eleve avec l'id {entity.EleveId} n'existe pas.");

        if (!await _anneeScolaireRepository.ExistsAsync(entity.AnneeScolaireId, ct).ConfigureAwait(false))
            throw new KeyNotFoundException($"L'annee scolaire avec l'id {entity.AnneeScolaireId} n'existe pas.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Dossier d'admission {Id} cree pour l'eleve {EleveId}.",
            created.Id, created.EleveId);

        var evt = new EntityCreatedEvent<DossierAdmission>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.dossieradmission", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(DossierAdmission entity, CancellationToken ct = default)
    {
        if (!StatutsDossierValides.Contains(entity.StatutDossier))
            throw new InvalidOperationException(
                $"Statut de dossier invalide : '{entity.StatutDossier}'.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Dossier d'admission {Id} mis a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<DossierAdmission>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.dossieradmission", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Dossier d'admission {Id} supprime.", id);

            var evt = new EntityDeletedEvent<DossierAdmission>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.dossieradmission", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
