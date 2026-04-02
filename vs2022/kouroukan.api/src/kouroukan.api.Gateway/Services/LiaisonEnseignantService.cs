using Dapper;
using GnDapper.Connection;
using Kouroukan.Api.Gateway.Models;

namespace Kouroukan.Api.Gateway.Services;

/// <summary>
/// Implementation du service de liaisons enseignant-etablissement.
/// </summary>
public sealed class LiaisonEnseignantService : ILiaisonEnseignantService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<LiaisonEnseignantService> _logger;

    public LiaisonEnseignantService(
        IDbConnectionFactory connectionFactory,
        ILogger<LiaisonEnseignantService> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<List<LiaisonEnseignantDto>> GetLiaisonsAsync(int? userId, int? companyId, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var conditions = new List<string> { "l.is_deleted = FALSE" };
        if (userId.HasValue)
            conditions.Add("l.user_id = @UserId");
        if (companyId.HasValue)
            conditions.Add("l.company_id = @CompanyId");

        var whereClause = string.Join(" AND ", conditions);

        var items = await connection.QueryAsync<LiaisonEnseignantDto>(
            $"""
            SELECT l.id, l.user_id AS UserId,
                   CONCAT(u.first_name, ' ', u.last_name) AS UserNom,
                   l.company_id AS CompanyId, c.name AS CompanyNom,
                   l.statut, l.identifiant, l.created_at AS CreatedAt,
                   l.accepted_at AS AcceptedAt, l.terminated_at AS TerminatedAt
            FROM auth.liaisons_enseignant l
            INNER JOIN auth.users u ON u.id = l.user_id
            INNER JOIN auth.companies c ON c.id = l.company_id
            WHERE {whereClause}
            ORDER BY l.created_at DESC
            """,
            new { UserId = userId, CompanyId = companyId });

        return items.AsList();
    }

    public async Task<LiaisonEnseignantDto> CreateLiaisonAsync(int userId, CreateLiaisonEnseignantRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        // Si un QR code est fourni, resoudre l'etablissement
        var companyId = request.CompanyId;
        if (!string.IsNullOrWhiteSpace(request.QrCode))
        {
            var resolved = await connection.ExecuteScalarAsync<int?>(
                """
                SELECT u.id
                FROM auth.qr_codes qr
                INNER JOIN auth.users u ON u.id = qr.user_id
                WHERE qr.code = @Code AND qr.is_deleted = FALSE
                  AND (qr.expires_at IS NULL OR qr.expires_at > NOW())
                """,
                new { Code = request.QrCode });

            if (!resolved.HasValue)
                throw new InvalidOperationException("QR code invalide ou expire.");
        }

        // Verifier qu'il n'y a pas deja une liaison active
        var existingLiaison = await connection.ExecuteScalarAsync<bool>(
            """
            SELECT EXISTS(
                SELECT 1 FROM auth.liaisons_enseignant
                WHERE user_id = @UserId AND company_id = @CompanyId
                  AND statut IN ('en_attente', 'acceptee')
                  AND is_deleted = FALSE
            )
            """,
            new { UserId = userId, CompanyId = companyId });

        if (existingLiaison)
            throw new InvalidOperationException("Une liaison active ou en attente existe deja.");

        var id = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO auth.liaisons_enseignant (user_id, company_id, identifiant, statut)
            VALUES (@UserId, @CompanyId, @Identifiant, 'en_attente')
            RETURNING id
            """,
            new { UserId = userId, CompanyId = companyId, request.Identifiant });

        _logger.LogInformation("Liaison enseignant {LiaisonId} creee (user={UserId}, company={CompanyId})", id, userId, companyId);

        return (await connection.QuerySingleAsync<LiaisonEnseignantDto>(
            """
            SELECT l.id, l.user_id AS UserId,
                   CONCAT(u.first_name, ' ', u.last_name) AS UserNom,
                   l.company_id AS CompanyId, c.name AS CompanyNom,
                   l.statut, l.identifiant, l.created_at AS CreatedAt,
                   l.accepted_at AS AcceptedAt, l.terminated_at AS TerminatedAt
            FROM auth.liaisons_enseignant l
            INNER JOIN auth.users u ON u.id = l.user_id
            INNER JOIN auth.companies c ON c.id = l.company_id
            WHERE l.id = @Id
            """,
            new { Id = id }));
    }

    public async Task AcceptLiaisonAsync(int id, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE auth.liaisons_enseignant
            SET statut = 'acceptee', accepted_at = NOW(), updated_at = NOW()
            WHERE id = @Id AND statut = 'en_attente' AND is_deleted = FALSE
            """,
            new { Id = id });

        if (affected == 0)
            throw new InvalidOperationException("Liaison introuvable ou non en attente.");

        // Ajouter l'enseignant a l'etablissement
        var liaison = await connection.QuerySingleAsync<(int UserId, int CompanyId)>(
            "SELECT user_id, company_id FROM auth.liaisons_enseignant WHERE id = @Id",
            new { Id = id });

        await connection.ExecuteAsync(
            """
            INSERT INTO auth.user_companies (user_id, company_id, role)
            VALUES (@UserId, @CompanyId, 'member')
            ON CONFLICT DO NOTHING
            """,
            new { liaison.UserId, liaison.CompanyId });

        // Assigner le role enseignant
        await connection.ExecuteAsync(
            """
            INSERT INTO auth.user_roles (user_id, role_id)
            SELECT @UserId, id FROM auth.roles WHERE name = 'enseignant' AND is_deleted = FALSE
            ON CONFLICT DO NOTHING
            """,
            new { liaison.UserId });

        _logger.LogInformation("Liaison enseignant {LiaisonId} acceptee", id);
    }

    public async Task RejectLiaisonAsync(int id, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE auth.liaisons_enseignant
            SET statut = 'rejetee', updated_at = NOW()
            WHERE id = @Id AND statut = 'en_attente' AND is_deleted = FALSE
            """,
            new { Id = id });

        if (affected == 0)
            throw new InvalidOperationException("Liaison introuvable ou non en attente.");

        _logger.LogInformation("Liaison enseignant {LiaisonId} rejetee", id);
    }

    public async Task TerminateLiaisonAsync(int id, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE auth.liaisons_enseignant
            SET statut = 'terminee', terminated_at = NOW(), updated_at = NOW()
            WHERE id = @Id AND statut = 'acceptee' AND is_deleted = FALSE
            """,
            new { Id = id });

        if (affected == 0)
            throw new InvalidOperationException("Liaison introuvable ou non acceptee.");

        _logger.LogInformation("Liaison enseignant {LiaisonId} terminee (historique conserve)", id);
    }

    public async Task ReintegrateLiaisonAsync(int id, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var affected = await connection.ExecuteAsync(
            """
            UPDATE auth.liaisons_enseignant
            SET statut = 'acceptee', terminated_at = NULL, accepted_at = NOW(), updated_at = NOW()
            WHERE id = @Id AND statut = 'terminee' AND is_deleted = FALSE
            """,
            new { Id = id });

        if (affected == 0)
            throw new InvalidOperationException("Liaison introuvable ou non terminee.");

        // Re-ajouter l'enseignant a l'etablissement
        var liaison = await connection.QuerySingleAsync<(int UserId, int CompanyId)>(
            "SELECT user_id, company_id FROM auth.liaisons_enseignant WHERE id = @Id",
            new { Id = id });

        await connection.ExecuteAsync(
            """
            INSERT INTO auth.user_companies (user_id, company_id, role)
            VALUES (@UserId, @CompanyId, 'member')
            ON CONFLICT DO NOTHING
            """,
            new { liaison.UserId, liaison.CompanyId });

        _logger.LogInformation("Liaison enseignant {LiaisonId} reintegree", id);
    }
}
