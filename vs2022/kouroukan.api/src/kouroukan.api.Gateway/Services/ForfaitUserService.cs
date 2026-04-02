using Dapper;
using GnDapper.Connection;
using Kouroukan.Api.Gateway.Models;

namespace Kouroukan.Api.Gateway.Services;

/// <summary>
/// Implementation du service de gestion des forfaits cote utilisateur.
/// </summary>
public sealed class ForfaitUserService : IForfaitUserService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<ForfaitUserService> _logger;

    public ForfaitUserService(
        IDbConnectionFactory connectionFactory,
        ILogger<ForfaitUserService> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<ForfaitStatusDto> GetStatusAsync(int? companyId, int? userId, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var status = await connection.QuerySingleOrDefaultAsync<ForfaitStatusDto>(
            """
            SELECT ab.id AS AbonnementId, f.nom AS ForfaitNom, f.code AS ForfaitCode,
                   f.type_cible AS TypeCible, f.est_gratuit AS EstGratuit,
                   f.limite_eleves AS LimiteEleves,
                   ab.date_debut AS DateDebut, ab.date_fin AS DateFin,
                   ab.date_essai_fin AS DateEssaiFin, ab.est_actif AS EstActif,
                   (ab.date_essai_fin IS NOT NULL AND ab.date_essai_fin > NOW()) AS EstEnEssai,
                   (ab.est_actif AND NOT f.est_gratuit) AS PeutResilier
            FROM auth.abonnements ab
            JOIN auth.forfaits f ON f.id = ab.forfait_id AND f.is_deleted = FALSE
            WHERE ab.is_deleted = FALSE AND ab.est_actif = TRUE
              AND ((@CompanyId IS NOT NULL AND ab.company_id = @CompanyId)
                OR (@UserId IS NOT NULL AND ab.user_id = @UserId))
            ORDER BY ab.created_at DESC
            LIMIT 1
            """,
            new { CompanyId = companyId, UserId = userId });

        if (status is null)
        {
            return new ForfaitStatusDto { EstActif = false };
        }

        // Pour les etablissements, compter les eleves actifs
        if (companyId.HasValue)
        {
            var studentCount = await connection.ExecuteScalarAsync<int>(
                """
                SELECT COUNT(*) FROM inscriptions.eleves e
                JOIN inscriptions.inscriptions i ON i.eleve_id = e.id AND i.is_deleted = FALSE
                WHERE e.is_deleted = FALSE AND i.company_id = @CompanyId
                  AND i.statut_inscription IN ('Validee', 'En attente')
                """,
                new { CompanyId = companyId });

            status.NombreElevesActuel = studentCount;
        }

        return status;
    }

    public async Task<List<ForfaitPlanDto>> GetAvailablePlansAsync(string typeCible, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var plans = await connection.QueryAsync<ForfaitPlanDto>(
            """
            SELECT id, code, nom, description, prix_mensuel AS PrixMensuel,
                   prix_vacances AS PrixVacances, periode_essai_jours AS PeriodeEssaiJours,
                   limite_eleves AS LimiteEleves, est_gratuit AS EstGratuit
            FROM auth.forfaits
            WHERE is_deleted = FALSE AND est_actif = TRUE AND type_cible = @TypeCible
            ORDER BY prix_mensuel ASC
            """,
            new { TypeCible = typeCible });

        return plans.AsList();
    }

    public async Task<AbonnementHistoryDto> SubscribeAsync(int? companyId, int? userId, SubscribeForfaitRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        // Verifier qu'il n'y a pas d'abonnement actif
        var existingCount = await connection.ExecuteScalarAsync<int>(
            """
            SELECT COUNT(*) FROM auth.abonnements
            WHERE is_deleted = FALSE AND est_actif = TRUE
              AND ((@CompanyId IS NOT NULL AND company_id = @CompanyId)
                OR (@UserId IS NOT NULL AND user_id = @UserId))
            """,
            new { CompanyId = companyId, UserId = userId });

        if (existingCount > 0)
        {
            throw new InvalidOperationException("Un abonnement actif existe deja.");
        }

        // Recuperer le forfait pour la periode d'essai
        var forfait = await connection.QuerySingleOrDefaultAsync<ForfaitPlanDto>(
            """
            SELECT id, code, nom, description, prix_mensuel AS PrixMensuel,
                   prix_vacances AS PrixVacances, periode_essai_jours AS PeriodeEssaiJours,
                   limite_eleves AS LimiteEleves, est_gratuit AS EstGratuit
            FROM auth.forfaits
            WHERE id = @ForfaitId AND is_deleted = FALSE AND est_actif = TRUE
            """,
            new { ForfaitId = request.ForfaitId });

        if (forfait is null)
        {
            throw new KeyNotFoundException("Forfait introuvable ou inactif.");
        }

        // Calculer la date de fin d'essai
        DateTime? dateEssaiFin = forfait.PeriodeEssaiJours > 0
            ? DateTime.UtcNow.AddDays(forfait.PeriodeEssaiJours)
            : null;

        // Inserer l'abonnement
        var abonnementId = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO auth.abonnements (forfait_id, company_id, user_id, date_debut, date_essai_fin, est_actif, created_at)
            VALUES (@ForfaitId, @CompanyId, @UserId, NOW(), @DateEssaiFin, TRUE, NOW())
            RETURNING id
            """,
            new
            {
                ForfaitId = request.ForfaitId,
                CompanyId = companyId,
                UserId = userId,
                DateEssaiFin = dateEssaiFin
            });

        _logger.LogInformation(
            "Abonnement {AbonnementId} cree pour forfait {ForfaitId} (company={CompanyId}, user={UserId})",
            abonnementId, request.ForfaitId, companyId, userId);

        return new AbonnementHistoryDto
        {
            Id = abonnementId,
            ForfaitNom = forfait.Nom,
            DateDebut = DateTime.UtcNow,
            DateFin = null,
            Montant = forfait.PrixMensuel,
            Statut = "actif",
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task CancelAsync(int abonnementId, int? companyId, int? userId, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        // Calculer la fin de la periode en cours (fin du mois courant)
        var dateFin = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)
            .AddMonths(1)
            .AddDays(-1);

        var affected = await connection.ExecuteAsync(
            """
            UPDATE auth.abonnements
            SET date_fin = @DateFin, updated_at = NOW()
            WHERE id = @AbonnementId AND is_deleted = FALSE AND est_actif = TRUE
              AND ((@CompanyId IS NOT NULL AND company_id = @CompanyId)
                OR (@UserId IS NOT NULL AND user_id = @UserId))
            """,
            new
            {
                DateFin = dateFin,
                AbonnementId = abonnementId,
                CompanyId = companyId,
                UserId = userId
            });

        if (affected == 0)
        {
            throw new KeyNotFoundException("Abonnement introuvable ou deja resilie.");
        }

        _logger.LogInformation("Abonnement {AbonnementId} resilie, fin prevue le {DateFin}", abonnementId, dateFin);
    }

    public async Task<List<AbonnementHistoryDto>> GetHistoryAsync(int? companyId, int? userId, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var history = await connection.QueryAsync<AbonnementHistoryDto>(
            """
            SELECT ab.id, f.nom AS ForfaitNom, ab.date_debut AS DateDebut,
                   ab.date_fin AS DateFin, f.prix_mensuel AS Montant,
                   CASE
                     WHEN ab.est_actif AND ab.date_fin IS NULL THEN 'actif'
                     WHEN ab.est_actif AND ab.date_fin > NOW() THEN 'résilie'
                     WHEN NOT ab.est_actif OR ab.date_fin <= NOW() THEN 'expire'
                   END AS Statut,
                   ab.created_at AS CreatedAt
            FROM auth.abonnements ab
            JOIN auth.forfaits f ON f.id = ab.forfait_id
            WHERE ab.is_deleted = FALSE
              AND ((@CompanyId IS NOT NULL AND ab.company_id = @CompanyId)
                OR (@UserId IS NOT NULL AND ab.user_id = @UserId))
            ORDER BY ab.created_at DESC
            """,
            new { CompanyId = companyId, UserId = userId });

        return history.AsList();
    }

    public async Task<QuotaCheckResult> CheckStudentQuotaAsync(int companyId, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        // Recuperer la limite d'eleves du forfait actif
        var limite = await connection.QuerySingleOrDefaultAsync<int?>(
            """
            SELECT f.limite_eleves
            FROM auth.abonnements ab
            JOIN auth.forfaits f ON f.id = ab.forfait_id AND f.is_deleted = FALSE
            WHERE ab.is_deleted = FALSE AND ab.est_actif = TRUE AND ab.company_id = @CompanyId
            ORDER BY ab.created_at DESC
            LIMIT 1
            """,
            new { CompanyId = companyId });

        // Compter les eleves actuels
        var nombreActuel = await connection.ExecuteScalarAsync<int>(
            """
            SELECT COUNT(*) FROM inscriptions.eleves e
            JOIN inscriptions.inscriptions i ON i.eleve_id = e.id AND i.is_deleted = FALSE
            WHERE e.is_deleted = FALSE AND i.company_id = @CompanyId
              AND i.statut_inscription IN ('Validee', 'En attente')
            """,
            new { CompanyId = companyId });

        var limiteAtteinte = limite.HasValue && nombreActuel >= limite.Value;

        return new QuotaCheckResult
        {
            LimiteAtteinte = limiteAtteinte,
            Limite = limite,
            NombreActuel = nombreActuel,
            Message = limiteAtteinte
                ? $"Limite de {limite} eleves atteinte ({nombreActuel}/{limite})."
                : null
        };
    }
}
