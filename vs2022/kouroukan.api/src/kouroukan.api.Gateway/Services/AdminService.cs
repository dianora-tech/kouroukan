using Dapper;
using GnDapper.Connection;
using Kouroukan.Api.Gateway.Models;

namespace Kouroukan.Api.Gateway.Services;

/// <summary>
/// Implementation du service d'administration de la plateforme.
/// </summary>
public sealed class AdminService : IAdminService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IDbConnectionFactory connectionFactory,
        ILogger<AdminService> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // FORFAITS
    // ═══════════════════════════════════════════════════════════════════════════

    public async Task<PagedResult<ForfaitDto>> GetForfaitsAsync(int page, int pageSize, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var offset = (page - 1) * pageSize;

        var totalCount = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM auth.forfaits WHERE is_deleted = FALSE");

        var items = await connection.QueryAsync<ForfaitDto>(
            """
            SELECT id, code, nom, description,
                   prix_mensuel AS PrixMensuel, prix_vacances AS PrixVacances,
                   periode_essai_jours AS PeriodeEssaiJours,
                   type_cible AS TypeCible, est_gratuit AS EstGratuit,
                   limite_eleves AS LimiteEleves,
                   est_actif AS EstActif, created_at AS CreatedAt
            FROM auth.forfaits
            WHERE is_deleted = FALSE
            ORDER BY created_at DESC
            LIMIT @PageSize OFFSET @Offset
            """,
            new { PageSize = pageSize, Offset = offset });

        return new PagedResult<ForfaitDto>
        {
            Items = items.AsList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ForfaitDto?> GetForfaitByIdAsync(int id, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<ForfaitDto>(
            """
            SELECT id, code, nom, description,
                   prix_mensuel AS PrixMensuel, prix_vacances AS PrixVacances,
                   periode_essai_jours AS PeriodeEssaiJours,
                   type_cible AS TypeCible, est_gratuit AS EstGratuit,
                   limite_eleves AS LimiteEleves,
                   est_actif AS EstActif, created_at AS CreatedAt
            FROM auth.forfaits
            WHERE id = @Id AND is_deleted = FALSE
            """,
            new { Id = id });
    }

    public async Task<ForfaitDto> CreateForfaitAsync(CreateForfaitRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var id = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO auth.forfaits (code, nom, description, prix_mensuel, prix_vacances,
                                       periode_essai_jours, type_cible, est_gratuit, limite_eleves)
            VALUES (@Code, @Nom, @Description, @PrixMensuel, @PrixVacances,
                    @PeriodeEssaiJours, @TypeCible, @EstGratuit, @LimiteEleves)
            RETURNING id
            """,
            new
            {
                request.Code,
                request.Nom,
                request.Description,
                request.PrixMensuel,
                request.PrixVacances,
                request.PeriodeEssaiJours,
                request.TypeCible,
                request.EstGratuit,
                request.LimiteEleves
            });

        _logger.LogInformation("Forfait {ForfaitId} '{Code}' cree", id, request.Code);

        return (await GetForfaitByIdAsync(id, ct))!;
    }

    public async Task UpdateForfaitAsync(int id, UpdateForfaitRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE auth.forfaits
            SET nom = @Nom, description = @Description,
                prix_mensuel = @PrixMensuel, prix_vacances = @PrixVacances,
                periode_essai_jours = @PeriodeEssaiJours,
                type_cible = @TypeCible, est_gratuit = @EstGratuit,
                limite_eleves = @LimiteEleves,
                est_actif = @EstActif, updated_at = NOW()
            WHERE id = @Id AND is_deleted = FALSE
            """,
            new
            {
                Id = id,
                request.Nom,
                request.Description,
                request.PrixMensuel,
                request.PrixVacances,
                request.PeriodeEssaiJours,
                request.TypeCible,
                request.EstGratuit,
                request.LimiteEleves,
                request.EstActif
            });

        if (affected == 0)
            throw new InvalidOperationException("Forfait introuvable.");

        _logger.LogInformation("Forfait {ForfaitId} mis a jour", id);
    }

    public async Task DeleteForfaitAsync(int id, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE auth.forfaits
            SET is_deleted = TRUE, updated_at = NOW()
            WHERE id = @Id AND is_deleted = FALSE
            """,
            new { Id = id });

        if (affected == 0)
            throw new InvalidOperationException("Forfait introuvable.");

        _logger.LogInformation("Forfait {ForfaitId} supprime (soft)", id);
    }

    public async Task UpdateForfaitTarifAsync(int id, UpdateForfaitTarifRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        // Inserer dans la table d'historique des tarifs
        await connection.ExecuteAsync(
            """
            INSERT INTO auth.forfait_tarifs (forfait_id, prix_mensuel, prix_vacances, date_effet)
            VALUES (@ForfaitId, @PrixMensuel, @PrixVacances, @DateEffet)
            """,
            new
            {
                ForfaitId = id,
                request.PrixMensuel,
                request.PrixVacances,
                request.DateEffet
            });

        // Mettre a jour le tarif courant sur le forfait
        await connection.ExecuteAsync(
            """
            UPDATE auth.forfaits
            SET prix_mensuel = @PrixMensuel, prix_vacances = @PrixVacances, updated_at = NOW()
            WHERE id = @Id AND is_deleted = FALSE
            """,
            new { Id = id, request.PrixMensuel, request.PrixVacances });

        _logger.LogInformation("Tarif du forfait {ForfaitId} mis a jour (effet: {DateEffet})", id, request.DateEffet);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // ABONNEMENTS
    // ═══════════════════════════════════════════════════════════════════════════

    public async Task<PagedResult<AbonnementDto>> GetAbonnementsAsync(int page, int pageSize, int? companyId, int? userId, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var offset = (page - 1) * pageSize;
        var conditions = new List<string> { "a.is_deleted = FALSE" };
        if (companyId.HasValue)
            conditions.Add("a.company_id = @CompanyId");
        if (userId.HasValue)
            conditions.Add("a.user_id = @UserId");

        var whereClause = string.Join(" AND ", conditions);

        var totalCount = await connection.ExecuteScalarAsync<int>(
            $"SELECT COUNT(*) FROM auth.abonnements a WHERE {whereClause}",
            new { CompanyId = companyId, UserId = userId });

        var items = await connection.QueryAsync<AbonnementDto>(
            $"""
            SELECT a.id, a.forfait_id AS ForfaitId, f.nom AS ForfaitNom,
                   a.company_id AS CompanyId, c.name AS CompanyNom,
                   a.user_id AS UserId,
                   CONCAT(u.first_name, ' ', u.last_name) AS UserNom,
                   a.date_debut AS DateDebut, a.date_fin AS DateFin,
                   a.date_essai_fin AS DateEssaiFin,
                   a.est_actif AS EstActif,
                   a.geste_commercial_id AS GesteCommercialId,
                   a.created_at AS CreatedAt
            FROM auth.abonnements a
            LEFT JOIN auth.forfaits f ON f.id = a.forfait_id
            LEFT JOIN auth.companies c ON c.id = a.company_id
            LEFT JOIN auth.users u ON u.id = a.user_id
            WHERE {whereClause}
            ORDER BY a.created_at DESC
            LIMIT @PageSize OFFSET @Offset
            """,
            new { CompanyId = companyId, UserId = userId, PageSize = pageSize, Offset = offset });

        return new PagedResult<AbonnementDto>
        {
            Items = items.AsList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<AbonnementDto> CreateAbonnementAsync(CreateAbonnementRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var id = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO auth.abonnements (forfait_id, company_id, user_id, date_debut, date_fin, date_essai_fin, geste_commercial_id)
            VALUES (@ForfaitId, @CompanyId, @UserId, @DateDebut, @DateFin, @DateEssaiFin, @GesteCommercialId)
            RETURNING id
            """,
            new
            {
                request.ForfaitId,
                request.CompanyId,
                request.UserId,
                request.DateDebut,
                request.DateFin,
                request.DateEssaiFin,
                request.GesteCommercialId
            });

        _logger.LogInformation("Abonnement {AbonnementId} cree", id);

        return (await connection.QuerySingleAsync<AbonnementDto>(
            """
            SELECT a.id, a.forfait_id AS ForfaitId, f.nom AS ForfaitNom,
                   a.company_id AS CompanyId, c.name AS CompanyNom,
                   a.user_id AS UserId,
                   CONCAT(u.first_name, ' ', u.last_name) AS UserNom,
                   a.date_debut AS DateDebut, a.date_fin AS DateFin,
                   a.date_essai_fin AS DateEssaiFin,
                   a.est_actif AS EstActif,
                   a.geste_commercial_id AS GesteCommercialId,
                   a.created_at AS CreatedAt
            FROM auth.abonnements a
            LEFT JOIN auth.forfaits f ON f.id = a.forfait_id
            LEFT JOIN auth.companies c ON c.id = a.company_id
            LEFT JOIN auth.users u ON u.id = a.user_id
            WHERE a.id = @Id
            """,
            new { Id = id }));
    }

    public async Task UpdateAbonnementAsync(int id, UpdateAbonnementRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE auth.abonnements
            SET date_fin = @DateFin, date_essai_fin = @DateEssaiFin,
                est_actif = @EstActif, geste_commercial_id = @GesteCommercialId,
                updated_at = NOW()
            WHERE id = @Id AND is_deleted = FALSE
            """,
            new { Id = id, request.DateFin, request.DateEssaiFin, request.EstActif, request.GesteCommercialId });

        if (affected == 0)
            throw new InvalidOperationException("Abonnement introuvable.");

        _logger.LogInformation("Abonnement {AbonnementId} mis a jour", id);
    }

    public async Task DeleteAbonnementAsync(int id, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE auth.abonnements
            SET is_deleted = TRUE, updated_at = NOW()
            WHERE id = @Id AND is_deleted = FALSE
            """,
            new { Id = id });

        if (affected == 0)
            throw new InvalidOperationException("Abonnement introuvable.");

        _logger.LogInformation("Abonnement {AbonnementId} supprime (soft)", id);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // GESTES COMMERCIAUX
    // ═══════════════════════════════════════════════════════════════════════════

    public async Task<List<GesteCommercialDto>> GetGestesCommerciauxAsync(string? typeCible, int? companyId, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var conditions = new List<string> { "g.is_deleted = FALSE" };
        if (!string.IsNullOrWhiteSpace(typeCible))
            conditions.Add("g.type_cible = @TypeCible");
        if (companyId.HasValue)
            conditions.Add("g.company_id = @CompanyId");

        var whereClause = string.Join(" AND ", conditions);

        var items = await connection.QueryAsync<GesteCommercialDto>(
            $"""
            SELECT g.id, g.nom, g.description,
                   g.type_cible AS TypeCible, g.cible_valeur AS CibleValeur,
                   g.forfait_id AS ForfaitId, f.nom AS ForfaitNom,
                   g.reduction_pourcent AS ReductionPourcent,
                   g.reduction_montant AS ReductionMontant,
                   g.date_debut AS DateDebut, g.date_fin AS DateFin,
                   g.est_actif AS EstActif,
                   g.company_id AS CompanyId, c.name AS CompanyNom,
                   g.created_at AS CreatedAt
            FROM auth.gestes_commerciaux g
            LEFT JOIN auth.companies c ON c.id = g.company_id
            LEFT JOIN auth.forfaits f ON f.id = g.forfait_id
            WHERE {whereClause}
            ORDER BY g.created_at DESC
            """,
            new { TypeCible = typeCible, CompanyId = companyId });

        return items.AsList();
    }

    public async Task<GesteCommercialDto> CreateGesteCommercialAsync(CreateGesteCommercialRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var id = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO auth.gestes_commerciaux (nom, description, type_cible, cible_valeur, forfait_id,
                                                  reduction_pourcent, reduction_montant, date_debut, date_fin, company_id)
            VALUES (@Nom, @Description, @TypeCible, @CibleValeur, @ForfaitId,
                    @ReductionPourcent, @ReductionMontant, @DateDebut, @DateFin, @CompanyId)
            RETURNING id
            """,
            new
            {
                request.Nom,
                request.Description,
                request.TypeCible,
                request.CibleValeur,
                request.ForfaitId,
                request.ReductionPourcent,
                request.ReductionMontant,
                request.DateDebut,
                request.DateFin,
                request.CompanyId
            });

        _logger.LogInformation("Geste commercial {GesteId} cree", id);

        return (await connection.QuerySingleAsync<GesteCommercialDto>(
            """
            SELECT g.id, g.nom, g.description,
                   g.type_cible AS TypeCible, g.cible_valeur AS CibleValeur,
                   g.forfait_id AS ForfaitId, f.nom AS ForfaitNom,
                   g.reduction_pourcent AS ReductionPourcent,
                   g.reduction_montant AS ReductionMontant,
                   g.date_debut AS DateDebut, g.date_fin AS DateFin,
                   g.est_actif AS EstActif,
                   g.company_id AS CompanyId, c.name AS CompanyNom,
                   g.created_at AS CreatedAt
            FROM auth.gestes_commerciaux g
            LEFT JOIN auth.companies c ON c.id = g.company_id
            LEFT JOIN auth.forfaits f ON f.id = g.forfait_id
            WHERE g.id = @Id
            """,
            new { Id = id }));
    }

    public async Task UpdateGesteCommercialAsync(int id, UpdateGesteCommercialRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE auth.gestes_commerciaux
            SET nom = @Nom, description = @Description,
                type_cible = @TypeCible, cible_valeur = @CibleValeur,
                forfait_id = @ForfaitId,
                reduction_pourcent = @ReductionPourcent, reduction_montant = @ReductionMontant,
                date_debut = @DateDebut, date_fin = @DateFin, est_actif = @EstActif
            WHERE id = @Id AND is_deleted = FALSE
            """,
            new
            {
                Id = id,
                request.Nom,
                request.Description,
                request.TypeCible,
                request.CibleValeur,
                request.ForfaitId,
                request.ReductionPourcent,
                request.ReductionMontant,
                request.DateDebut,
                request.DateFin,
                request.EstActif
            });

        if (affected == 0)
            throw new InvalidOperationException("Geste commercial introuvable.");

        _logger.LogInformation("Geste commercial {GesteId} mis a jour", id);
    }

    public async Task DeleteGesteCommercialAsync(int id, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE auth.gestes_commerciaux
            SET is_deleted = TRUE
            WHERE id = @Id AND is_deleted = FALSE
            """,
            new { Id = id });

        if (affected == 0)
            throw new InvalidOperationException("Geste commercial introuvable.");

        _logger.LogInformation("Geste commercial {GesteId} supprime (soft)", id);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // EMAIL CONFIG
    // ═══════════════════════════════════════════════════════════════════════════

    public async Task<EmailConfigDto?> GetEmailConfigAsync(CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<EmailConfigDto>(
            """
            SELECT id, smtp_host AS SmtpHost, smtp_port AS SmtpPort, smtp_user AS SmtpUser,
                   email_expediteur AS EmailExpediteur, nom_expediteur AS NomExpediteur,
                   est_actif AS EstActif, updated_at AS UpdatedAt
            FROM support.email_config
            LIMIT 1
            """);
    }

    public async Task UpdateEmailConfigAsync(UpdateEmailConfigRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var exists = await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM support.email_config)");

        if (exists)
        {
            var sql = string.IsNullOrWhiteSpace(request.SmtpPassword)
                ? """
                  UPDATE support.email_config
                  SET smtp_host = @SmtpHost, smtp_port = @SmtpPort, smtp_user = @SmtpUser,
                      email_expediteur = @EmailExpediteur, nom_expediteur = @NomExpediteur,
                      est_actif = TRUE, updated_at = NOW()
                  """
                : """
                  UPDATE support.email_config
                  SET smtp_host = @SmtpHost, smtp_port = @SmtpPort, smtp_user = @SmtpUser,
                      smtp_password = @SmtpPassword,
                      email_expediteur = @EmailExpediteur, nom_expediteur = @NomExpediteur,
                      est_actif = TRUE, updated_at = NOW()
                  """;

            await connection.ExecuteAsync(sql, new
            {
                request.SmtpHost,
                request.SmtpPort,
                request.SmtpUser,
                request.SmtpPassword,
                request.EmailExpediteur,
                request.NomExpediteur
            });
        }
        else
        {
            await connection.ExecuteAsync(
                """
                INSERT INTO support.email_config (smtp_host, smtp_port, smtp_user, smtp_password,
                                                  email_expediteur, nom_expediteur, est_actif)
                VALUES (@SmtpHost, @SmtpPort, @SmtpUser, @SmtpPassword, @EmailExpediteur, @NomExpediteur, TRUE)
                """,
                new
                {
                    request.SmtpHost,
                    request.SmtpPort,
                    request.SmtpUser,
                    SmtpPassword = request.SmtpPassword ?? string.Empty,
                    request.EmailExpediteur,
                    request.NomExpediteur
                });
        }

        _logger.LogInformation("Configuration email mise a jour");
    }

    public async Task<bool> SendTestEmailAsync(TestEmailRequest request, CancellationToken ct = default)
    {
        // Charger la config SMTP
        var config = await GetEmailConfigAsync(ct);
        if (config is null)
            throw new InvalidOperationException("Aucune configuration email trouvee.");

        using var connection = _connectionFactory.CreateConnection();

        // Recuperer le mot de passe SMTP
        var smtpPassword = await connection.ExecuteScalarAsync<string>(
            "SELECT smtp_password FROM support.email_config LIMIT 1");

        try
        {
            using var smtpClient = new System.Net.Mail.SmtpClient(config.SmtpHost, config.SmtpPort)
            {
                Credentials = new System.Net.NetworkCredential(config.SmtpUser, smtpPassword),
                EnableSsl = true
            };

            var message = new System.Net.Mail.MailMessage(
                new System.Net.Mail.MailAddress(config.EmailExpediteur, config.NomExpediteur),
                new System.Net.Mail.MailAddress(request.To))
            {
                Subject = "Kouroukan - Email de test",
                Body = "Ceci est un email de test envoye depuis la plateforme Kouroukan.",
                IsBodyHtml = false
            };

            await smtpClient.SendMailAsync(message, ct);
            _logger.LogInformation("Email de test envoye a {To}", request.To);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi de l'email de test a {To}", request.To);
            return false;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // SMS CONFIG
    // ═══════════════════════════════════════════════════════════════════════════

    public async Task<SmsConfigDto?> GetSmsConfigAsync(CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<SmsConfigDto>(
            """
            SELECT id, api_key AS ApiKey, api_secret AS ApiSecret,
                   sender_name AS SenderName,
                   est_actif AS EstActif, solde_actuel AS SoldeActuel,
                   derniere_synchro AS DerniereSynchro,
                   updated_at AS UpdatedAt
            FROM support.sms_config
            LIMIT 1
            """);
    }

    public async Task UpdateSmsConfigAsync(UpdateSmsConfigRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var exists = await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM support.sms_config)");

        if (exists)
        {
            await connection.ExecuteAsync(
                """
                UPDATE support.sms_config
                SET api_key = @ApiKey, api_secret = @ApiSecret,
                    sender_name = @SenderName, est_actif = TRUE, updated_at = NOW()
                """,
                new { request.ApiKey, request.ApiSecret, request.SenderName });
        }
        else
        {
            await connection.ExecuteAsync(
                """
                INSERT INTO support.sms_config (api_key, api_secret, sender_name, est_actif)
                VALUES (@ApiKey, @ApiSecret, @SenderName, TRUE)
                """,
                new { request.ApiKey, request.ApiSecret, request.SenderName });
        }

        _logger.LogInformation("Configuration SMS mise a jour");
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // COMPTES MOBILE MONEY
    // ═══════════════════════════════════════════════════════════════════════════

    public async Task<List<CompteMobileDto>> GetComptesMobileAsync(CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var items = await connection.QueryAsync<CompteMobileDto>(
            """
            SELECT id, operateur, numero_compte AS NumeroCompte,
                   code_marchand AS CodeMarchand, libelle,
                   est_actif AS EstActif, created_at AS CreatedAt
            FROM finances.comptes_admin
            WHERE is_deleted = FALSE
            ORDER BY created_at DESC
            """);

        return items.AsList();
    }

    public async Task<CompteMobileDto> CreateCompteMobileAsync(CreateCompteMobileRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var id = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO finances.comptes_admin (operateur, numero_compte, code_marchand, libelle)
            VALUES (@Operateur, @NumeroCompte, @CodeMarchand, @Libelle)
            RETURNING id
            """,
            new { request.Operateur, request.NumeroCompte, request.CodeMarchand, request.Libelle });

        _logger.LogInformation("Compte Mobile Money {CompteId} cree ({Operateur})", id, request.Operateur);

        return (await connection.QuerySingleAsync<CompteMobileDto>(
            """
            SELECT id, operateur, numero_compte AS NumeroCompte,
                   code_marchand AS CodeMarchand, libelle,
                   est_actif AS EstActif, created_at AS CreatedAt
            FROM finances.comptes_admin
            WHERE id = @Id
            """,
            new { Id = id }));
    }

    public async Task UpdateCompteMobileAsync(int id, UpdateCompteMobileRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE finances.comptes_admin
            SET operateur = @Operateur, numero_compte = @NumeroCompte,
                code_marchand = @CodeMarchand, libelle = @Libelle,
                est_actif = @EstActif, updated_at = NOW()
            WHERE id = @Id AND is_deleted = FALSE
            """,
            new { Id = id, request.Operateur, request.NumeroCompte, request.CodeMarchand, request.Libelle, request.EstActif });

        if (affected == 0)
            throw new InvalidOperationException("Compte Mobile Money introuvable.");

        _logger.LogInformation("Compte Mobile Money {CompteId} mis a jour", id);
    }

    public async Task DeleteCompteMobileAsync(int id, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE finances.comptes_admin
            SET is_deleted = TRUE, updated_at = NOW()
            WHERE id = @Id AND is_deleted = FALSE
            """,
            new { Id = id });

        if (affected == 0)
            throw new InvalidOperationException("Compte Mobile Money introuvable.");

        _logger.LogInformation("Compte Mobile Money {CompteId} supprime (soft)", id);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // CONTENU IA
    // ═══════════════════════════════════════════════════════════════════════════

    public async Task<List<ContenuIaDto>> GetContenusIaAsync(string? rubrique, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var whereClause = "is_deleted = FALSE";
        if (!string.IsNullOrWhiteSpace(rubrique))
            whereClause += " AND rubrique = @Rubrique";

        var items = await connection.QueryAsync<ContenuIaDto>(
            $"""
            SELECT id, rubrique, titre, contenu,
                   est_actif AS EstActif, ordre AS Ordre,
                   created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM support.contenu_ia
            WHERE {whereClause}
            ORDER BY ordre ASC, created_at DESC
            """,
            new { Rubrique = rubrique });

        return items.AsList();
    }

    public async Task<ContenuIaDto?> GetContenuIaByIdAsync(int id, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<ContenuIaDto>(
            """
            SELECT id, rubrique, titre, contenu,
                   est_actif AS EstActif, ordre AS Ordre,
                   created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM support.contenu_ia
            WHERE id = @Id AND is_deleted = FALSE
            """,
            new { Id = id });
    }

    public async Task<ContenuIaDto> CreateContenuIaAsync(CreateContenuIaRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var id = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO support.contenu_ia (rubrique, titre, contenu, ordre)
            VALUES (@Rubrique, @Titre, @Contenu, @Ordre)
            RETURNING id
            """,
            new { request.Rubrique, request.Titre, request.Contenu, request.Ordre });

        _logger.LogInformation("Contenu IA {ContenuId} cree (rubrique: {Rubrique})", id, request.Rubrique);

        return (await GetContenuIaByIdAsync(id, ct))!;
    }

    public async Task UpdateContenuIaAsync(int id, UpdateContenuIaRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE support.contenu_ia
            SET rubrique = @Rubrique, titre = @Titre, contenu = @Contenu,
                ordre = @Ordre, est_actif = @EstActif, updated_at = NOW()
            WHERE id = @Id AND is_deleted = FALSE
            """,
            new { Id = id, request.Rubrique, request.Titre, request.Contenu, request.Ordre, request.EstActif });

        if (affected == 0)
            throw new InvalidOperationException("Contenu IA introuvable.");

        _logger.LogInformation("Contenu IA {ContenuId} mis a jour", id);
    }

    public async Task DeleteContenuIaAsync(int id, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE support.contenu_ia
            SET is_deleted = TRUE, updated_at = NOW()
            WHERE id = @Id AND is_deleted = FALSE
            """,
            new { Id = id });

        if (affected == 0)
            throw new InvalidOperationException("Contenu IA introuvable.");

        _logger.LogInformation("Contenu IA {ContenuId} supprime (soft)", id);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // STATISTIQUES FORFAITS
    // ═══════════════════════════════════════════════════════════════════════════

    public async Task<ForfaitStatsDto> GetForfaitStatsAsync(CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var raw = await connection.QuerySingleAsync<ForfaitStatsRaw>(
            """
            SELECT
              (SELECT COUNT(DISTINCT uc.company_id) FROM auth.user_companies uc WHERE uc.is_deleted = FALSE) AS TotalEtablissements,
              (SELECT COUNT(DISTINCT u.id) FROM auth.users u JOIN auth.user_roles ur ON ur.user_id = u.id
               JOIN auth.roles r ON r.id = ur.role_id WHERE r.name = 'enseignant' AND u.is_deleted = FALSE AND u.is_active = TRUE) AS TotalEnseignants,
              (SELECT COUNT(DISTINCT u.id) FROM auth.users u JOIN auth.user_roles ur ON ur.user_id = u.id
               JOIN auth.roles r ON r.id = ur.role_id WHERE r.name = 'parent' AND u.is_deleted = FALSE AND u.is_active = TRUE) AS TotalParents,
              (SELECT COUNT(DISTINCT ab.company_id) FROM auth.abonnements ab
               JOIN auth.forfaits f ON f.id = ab.forfait_id WHERE f.type_cible = 'etablissement'
               AND ab.est_actif = TRUE AND ab.is_deleted = FALSE AND NOT f.est_gratuit) AS EtablissementsAvecForfait,
              (SELECT COUNT(DISTINCT ab.user_id) FROM auth.abonnements ab
               JOIN auth.forfaits f ON f.id = ab.forfait_id WHERE f.type_cible = 'enseignant'
               AND ab.est_actif = TRUE AND ab.is_deleted = FALSE) AS EnseignantsAvecForfait,
              (SELECT COUNT(DISTINCT ab.user_id) FROM auth.abonnements ab
               JOIN auth.forfaits f ON f.id = ab.forfait_id WHERE f.type_cible = 'parent'
               AND ab.est_actif = TRUE AND ab.is_deleted = FALSE) AS ParentsAvecForfait
            """);

        return new ForfaitStatsDto
        {
            TotalEtablissements = (int)raw.TotalEtablissements,
            EtablissementsAvecForfait = (int)raw.EtablissementsAvecForfait,
            TauxEtablissements = raw.TotalEtablissements > 0
                ? Math.Round((decimal)raw.EtablissementsAvecForfait / raw.TotalEtablissements * 100, 1)
                : 0,
            TotalEnseignants = (int)raw.TotalEnseignants,
            EnseignantsAvecForfait = (int)raw.EnseignantsAvecForfait,
            TauxEnseignants = raw.TotalEnseignants > 0
                ? Math.Round((decimal)raw.EnseignantsAvecForfait / raw.TotalEnseignants * 100, 1)
                : 0,
            TotalParents = (int)raw.TotalParents,
            ParentsAvecForfait = (int)raw.ParentsAvecForfait,
            TauxParents = raw.TotalParents > 0
                ? Math.Round((decimal)raw.ParentsAvecForfait / raw.TotalParents * 100, 1)
                : 0
        };
    }

    /// <summary>DTO interne pour le mapping des COUNT (PostgreSQL retourne bigint/long).</summary>
    private sealed class ForfaitStatsRaw
    {
        public long TotalEtablissements { get; set; }
        public long EtablissementsAvecForfait { get; set; }
        public long TotalEnseignants { get; set; }
        public long EnseignantsAvecForfait { get; set; }
        public long TotalParents { get; set; }
        public long ParentsAvecForfait { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // ETABLISSEMENTS
    // ═══════════════════════════════════════════════════════════════════════════

    public async Task<PagedResult<AdminEtablissementDto>> GetEtablissementsAsync(int page, int pageSize, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var offset = (page - 1) * pageSize;

        var totalCount = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM auth.companies WHERE is_deleted = FALSE");

        var items = await connection.QueryAsync<AdminEtablissementDto>(
            """
            SELECT c.id, c.name, c.email, c.phone_number AS PhoneNumber, c.address,
                   c.region_code AS RegionCode, c.prefecture_code AS PrefectureCode,
                   c.sous_prefecture_code AS SousPrefectureCode,
                   r.name AS RegionName, p.name AS PrefectureName, sp.name AS SousPrefectureName,
                   c.modules, NOT c.is_deleted AS IsActive, c.created_at AS CreatedAt,
                   (SELECT COUNT(*) FROM auth.user_companies uc
                    WHERE uc.company_id = c.id AND uc.is_deleted = FALSE) AS UserCount,
                   (SELECT f.nom FROM auth.abonnements ab
                    JOIN auth.forfaits f ON f.id = ab.forfait_id
                    WHERE ab.company_id = c.id
                      AND ab.is_deleted = FALSE AND ab.est_actif = TRUE
                    LIMIT 1) AS ForfaitNom,
                   (SELECT u.first_name || ' ' || u.last_name
                    FROM auth.user_companies uc2
                    JOIN auth.users u ON u.id = uc2.user_id AND u.is_deleted = FALSE
                    WHERE uc2.company_id = c.id AND uc2.role = 'owner' AND uc2.is_deleted = FALSE
                    LIMIT 1) AS DirecteurNom
            FROM auth.companies c
            LEFT JOIN geo.regions r ON r.code = c.region_code AND r.is_deleted = FALSE
            LEFT JOIN geo.prefectures p ON p.code = c.prefecture_code AND p.is_deleted = FALSE
            LEFT JOIN geo.sous_prefectures sp ON sp.code = c.sous_prefecture_code AND sp.is_deleted = FALSE
            WHERE c.is_deleted = FALSE
            ORDER BY c.created_at DESC
            LIMIT @PageSize OFFSET @Offset
            """,
            new { PageSize = pageSize, Offset = offset });

        return new PagedResult<AdminEtablissementDto>
        {
            Items = items.AsList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<AdminEtablissementDetailDto?> GetEtablissementByIdAsync(int id, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<AdminEtablissementDetailDto>(
            """
            SELECT c.id, c.name, c.email, c.phone_number AS PhoneNumber, c.address,
                   c.region_code AS RegionCode, c.prefecture_code AS PrefectureCode,
                   c.sous_prefecture_code AS SousPrefectureCode,
                   r.name AS RegionName, p.name AS PrefectureName, sp.name AS SousPrefectureName,
                   c.modules, NOT c.is_deleted AS IsActive, c.created_at AS CreatedAt,
                   (SELECT COUNT(*) FROM auth.user_companies uc
                    WHERE uc.company_id = c.id AND uc.is_deleted = FALSE) AS UserCount,
                   (SELECT f.nom FROM auth.abonnements ab
                    JOIN auth.forfaits f ON f.id = ab.forfait_id
                    WHERE ab.company_id = c.id
                      AND ab.is_deleted = FALSE AND ab.est_actif = TRUE
                    LIMIT 1) AS ForfaitNom,
                   (SELECT CONCAT(u.first_name, ' ', u.last_name)
                    FROM auth.users u
                    JOIN auth.user_companies uc ON uc.user_id = u.id
                    WHERE uc.company_id = c.id AND uc.role = 'owner'
                      AND uc.is_deleted = FALSE AND u.is_deleted = FALSE
                    LIMIT 1) AS DirecteurNom
            FROM auth.companies c
            LEFT JOIN geo.regions r ON r.code = c.region_code AND r.is_deleted = FALSE
            LEFT JOIN geo.prefectures p ON p.code = c.prefecture_code AND p.is_deleted = FALSE
            LEFT JOIN geo.sous_prefectures sp ON sp.code = c.sous_prefecture_code AND sp.is_deleted = FALSE
            WHERE c.id = @Id AND c.is_deleted = FALSE
            """,
            new { Id = id });
    }

    public async Task<AdminEtablissementDetailDto> UpdateEtablissementAsync(int id, UpdateEtablissementRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE auth.companies
            SET name = @Name, email = @Email, phone_number = @PhoneNumber,
                address = @Address, region_code = @RegionCode,
                prefecture_code = @PrefectureCode, sous_prefecture_code = @SousPrefectureCode,
                modules = @Modules::text[], updated_at = NOW()
            WHERE id = @Id AND is_deleted = FALSE
            """,
            new
            {
                Id = id,
                request.Name,
                request.Email,
                request.PhoneNumber,
                request.Address,
                request.RegionCode,
                request.PrefectureCode,
                request.SousPrefectureCode,
                Modules = request.Modules,
            });

        if (affected == 0)
            throw new InvalidOperationException("Etablissement introuvable.");

        _logger.LogInformation("Etablissement {CompanyId} mis a jour par l'admin", id);

        var updated = await GetEtablissementByIdAsync(id, ct);
        return updated!;
    }

    public async Task DeleteEtablissementAsync(int id, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE auth.companies
            SET is_deleted = TRUE, deleted_at = NOW(), deleted_by = 'admin'
            WHERE id = @Id AND is_deleted = FALSE
            """,
            new { Id = id });

        if (affected == 0)
            throw new InvalidOperationException("Etablissement introuvable.");

        _logger.LogInformation("Etablissement {CompanyId} supprime par l'admin", id);
    }
}
