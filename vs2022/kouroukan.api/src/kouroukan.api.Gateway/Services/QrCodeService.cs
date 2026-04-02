using System.Security.Cryptography;
using Dapper;
using GnDapper.Connection;
using Kouroukan.Api.Gateway.Models;

namespace Kouroukan.Api.Gateway.Services;

/// <summary>
/// Implementation du service de QR codes utilisateur.
/// </summary>
public sealed class QrCodeService : IQrCodeService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<QrCodeService> _logger;

    public QrCodeService(
        IDbConnectionFactory connectionFactory,
        ILogger<QrCodeService> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<QrCodeDto> GetOrCreateQrCodeAsync(int userId, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        // Chercher un QR code existant et valide
        var existing = await connection.QuerySingleOrDefaultAsync<QrCodeDto>(
            """
            SELECT id, user_id AS UserId, code, created_at AS CreatedAt, expires_at AS ExpiresAt
            FROM auth.qr_codes
            WHERE user_id = @UserId AND is_deleted = FALSE
              AND (expires_at IS NULL OR expires_at > NOW())
            ORDER BY created_at DESC
            LIMIT 1
            """,
            new { UserId = userId });

        if (existing is not null)
            return existing;

        // Generer un nouveau code unique
        var code = GenerateUniqueCode();

        var id = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO auth.qr_codes (user_id, code)
            VALUES (@UserId, @Code)
            RETURNING id
            """,
            new { UserId = userId, Code = code });

        _logger.LogInformation("QR code genere pour l'utilisateur {UserId}", userId);

        return new QrCodeDto
        {
            Id = id,
            UserId = userId,
            Code = code,
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task<QrCodeResolvedDto?> ResolveQrCodeAsync(string code, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<QrCodeResolvedDto>(
            """
            SELECT u.id AS UserId, u.first_name AS FirstName, u.last_name AS LastName,
                   u.avatar_url AS AvatarUrl,
                   (SELECT r.name FROM auth.user_roles ur
                    JOIN auth.roles r ON r.id = ur.role_id
                    WHERE ur.user_id = u.id AND ur.is_deleted = FALSE
                    LIMIT 1) AS Role
            FROM auth.qr_codes qr
            INNER JOIN auth.users u ON u.id = qr.user_id
            WHERE qr.code = @Code AND qr.is_deleted = FALSE
              AND (qr.expires_at IS NULL OR qr.expires_at > NOW())
              AND u.is_deleted = FALSE AND u.is_active = TRUE
            """,
            new { Code = code });
    }

    private static string GenerateUniqueCode()
    {
        // Generer un code aleatoire de 12 caracteres alphanumeriques
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        return new string(RandomNumberGenerator.GetBytes(12)
            .Select(b => chars[b % chars.Length])
            .ToArray());
    }
}
