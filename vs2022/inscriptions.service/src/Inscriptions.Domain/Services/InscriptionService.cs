using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Inscriptions.Domain.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Inscriptions.Domain.Services;

/// <summary>
/// Service metier pour les inscriptions.
/// </summary>
public sealed class InscriptionService : IInscriptionService
{
    private const string Exchange = "kouroukan.events";

    private static readonly string[] StatutsInscriptionValides = ["EnAttente", "Validee", "Annulee"];
    private static readonly string[] TypesEtablissementValides =
        ["Public", "PriveLaique", "PriveFrancoArabe", "Communautaire", "PriveCatholique", "PriveProtestant"];
    private static readonly string[] SeriesBacValides = ["SE", "SM", "SS", "FA"];

    private readonly IInscriptionRepository _repository;
    private readonly IEleveRepository _eleveRepository;
    private readonly IAnneeScolaireRepository _anneeScolaireRepository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<InscriptionService> _logger;

    public InscriptionService(
        IInscriptionRepository repository,
        IEleveRepository eleveRepository,
        IAnneeScolaireRepository anneeScolaireRepository,
        IMessagePublisher publisher,
        ILogger<InscriptionService> logger)
    {
        _repository = repository;
        _eleveRepository = eleveRepository;
        _anneeScolaireRepository = anneeScolaireRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Inscription?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Inscription>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<Inscription>> GetPagedAsync(
        int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default)
    {
        return await _repository.GetPagedAsync(page, pageSize, search, typeId, orderBy, ct).ConfigureAwait(false);
    }

    public async Task<Inscription> CreateAsync(Inscription entity, CancellationToken ct = default)
    {
        if (!StatutsInscriptionValides.Contains(entity.StatutInscription))
            throw new InvalidOperationException(
                $"Statut d'inscription invalide : '{entity.StatutInscription}'.");

        if (entity.TypeEtablissement is not null && !TypesEtablissementValides.Contains(entity.TypeEtablissement))
            throw new InvalidOperationException(
                $"Type d'etablissement invalide : '{entity.TypeEtablissement}'.");

        if (entity.SerieBac is not null && !SeriesBacValides.Contains(entity.SerieBac))
            throw new InvalidOperationException(
                $"Serie du bac invalide : '{entity.SerieBac}'. Valeurs acceptees : {string.Join(", ", SeriesBacValides)}");

        if (!await _eleveRepository.ExistsAsync(entity.EleveId, ct).ConfigureAwait(false))
            throw new KeyNotFoundException($"L'eleve avec l'id {entity.EleveId} n'existe pas.");

        if (!await _anneeScolaireRepository.ExistsAsync(entity.AnneeScolaireId, ct).ConfigureAwait(false))
            throw new KeyNotFoundException($"L'annee scolaire avec l'id {entity.AnneeScolaireId} n'existe pas.");

        var created = await _repository.AddAsync(entity, ct).ConfigureAwait(false);

        _logger.LogInformation("Inscription {Id} creee pour l'eleve {EleveId} dans la classe {ClasseId}.",
            created.Id, created.EleveId, created.ClasseId);

        var evt = new EntityCreatedEvent<Inscription>(created) { EntityId = created.Id, UserId = created.UserId };
        await _publisher.PublishAsync(evt, Exchange, "entity.created.inscription", cancellationToken: ct)
            .ConfigureAwait(false);

        return created;
    }

    public async Task<bool> UpdateAsync(Inscription entity, CancellationToken ct = default)
    {
        if (!StatutsInscriptionValides.Contains(entity.StatutInscription))
            throw new InvalidOperationException(
                $"Statut d'inscription invalide : '{entity.StatutInscription}'.");

        if (entity.TypeEtablissement is not null && !TypesEtablissementValides.Contains(entity.TypeEtablissement))
            throw new InvalidOperationException(
                $"Type d'etablissement invalide : '{entity.TypeEtablissement}'.");

        if (entity.SerieBac is not null && !SeriesBacValides.Contains(entity.SerieBac))
            throw new InvalidOperationException(
                $"Serie du bac invalide : '{entity.SerieBac}'.");

        var result = await _repository.UpdateAsync(entity, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Inscription {Id} mise a jour.", entity.Id);

            var evt = new EntityUpdatedEvent<Inscription>(entity) { EntityId = entity.Id, UserId = entity.UserId };
            await _publisher.PublishAsync(evt, Exchange, "entity.updated.inscription", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var result = await _repository.DeleteAsync(id, ct).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("Inscription {Id} supprimee.", id);

            var evt = new EntityDeletedEvent<Inscription>(id) { EntityId = id };
            await _publisher.PublishAsync(evt, Exchange, "entity.deleted.inscription", cancellationToken: ct)
                .ConfigureAwait(false);
        }

        return result;
    }
}
