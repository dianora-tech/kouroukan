using System.Security.Cryptography;
using Dapper;
using GnDapper.Connection;
using GnSecurity.Hashing;
using Kouroukan.Api.Gateway.Models;

namespace Kouroukan.Api.Gateway.Services;

public sealed class UserService : IUserService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IDbConnectionFactory connectionFactory,
        IPasswordHasher passwordHasher,
        ILogger<UserService> logger)
    {
        _connectionFactory = connectionFactory;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<CreateUserResultDto> CreateUserAsync(int directorId, CreateUserRequest request, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        // Get director's company
        var companyId = await connection.ExecuteScalarAsync<int?>(
            """
            SELECT company_id FROM auth.user_companies
            WHERE user_id = @DirectorId AND role = 'owner' AND is_deleted = FALSE
            LIMIT 1
            """,
            new { DirectorId = directorId });

        if (!companyId.HasValue)
            throw new InvalidOperationException("Etablissement introuvable pour ce directeur.");

        // If linking an existing user (fondateur multi-establishment)
        if (request.ExistingUserId.HasValue)
        {
            var existingUser = await connection.ExecuteScalarAsync<bool>(
                "SELECT EXISTS(SELECT 1 FROM auth.users WHERE id = @Id AND is_deleted = FALSE)",
                new { Id = request.ExistingUserId.Value });

            if (!existingUser)
                throw new InvalidOperationException("Utilisateur introuvable.");

            // Link user to company
            await connection.ExecuteAsync(
                """
                INSERT INTO auth.user_companies (user_id, company_id, role)
                VALUES (@UserId, @CompanyId, @Role)
                ON CONFLICT DO NOTHING
                """,
                new { UserId = request.ExistingUserId.Value, CompanyId = companyId.Value, Role = request.Role });

            // Assign role if not already assigned
            await connection.ExecuteAsync(
                """
                INSERT INTO auth.user_roles (user_id, role_id)
                SELECT @UserId, id FROM auth.roles WHERE name = @Role AND is_deleted = FALSE
                ON CONFLICT DO NOTHING
                """,
                new { UserId = request.ExistingUserId.Value, Role = request.Role });

            _logger.LogInformation("Utilisateur existant {UserId} lie a l'etablissement {CompanyId} avec le role {Role}",
                request.ExistingUserId.Value, companyId.Value, request.Role);

            return new CreateUserResultDto { UserId = request.ExistingUserId.Value, TemporaryPassword = "" };
        }

        // Check phone uniqueness
        var phoneExists = await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM auth.users WHERE phone_number = @Phone AND is_deleted = FALSE)",
            new { Phone = request.PhoneNumber });

        if (phoneExists)
            throw new ConflictException("Un compte avec ce numero de telephone existe deja.");

        // Check email uniqueness
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailExists = await connection.ExecuteScalarAsync<bool>(
                "SELECT EXISTS(SELECT 1 FROM auth.users WHERE email = @Email AND is_deleted = FALSE)",
                new { Email = request.Email });

            if (emailExists)
                throw new ConflictException("Un compte avec cet email existe deja.");
        }

        // Generate temporary password
        var tempPassword = GeneratePassword(12);
        var passwordHash = await _passwordHasher.HashAsync(tempPassword, ct);

        var effectiveEmail = !string.IsNullOrWhiteSpace(request.Email)
            ? request.Email
            : $"{request.PhoneNumber.Replace("+", "").Replace(" ", "")}@kouroukan.gn";

        // Create user
        var userId = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO auth.users (first_name, last_name, email, phone_number, password_hash, is_active, must_change_password, created_by)
            VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @PasswordHash, TRUE, TRUE, @CreatedBy)
            RETURNING id
            """,
            new
            {
                request.FirstName,
                request.LastName,
                Email = effectiveEmail,
                request.PhoneNumber,
                PasswordHash = passwordHash,
                CreatedBy = directorId
            });

        // Assign role
        await connection.ExecuteAsync(
            """
            INSERT INTO auth.user_roles (user_id, role_id)
            SELECT @UserId, id FROM auth.roles WHERE name = @Role AND is_deleted = FALSE
            """,
            new { UserId = userId, Role = request.Role });

        // Link to company
        await connection.ExecuteAsync(
            """
            INSERT INTO auth.user_companies (user_id, company_id, role)
            VALUES (@UserId, @CompanyId, 'member')
            """,
            new { UserId = userId, CompanyId = companyId.Value });

        _logger.LogInformation("Utilisateur {UserId} cree par le directeur {DirectorId} avec le role {Role}",
            userId, directorId, request.Role);

        return new CreateUserResultDto { UserId = userId, TemporaryPassword = tempPassword };
    }

    public async Task<List<UserListItemDto>> GetUsersForCompanyAsync(int directorId, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var users = await connection.QueryAsync<UserListItemDto>(
            """
            SELECT u.id, u.first_name AS FirstName, u.last_name AS LastName,
                   u.email, u.phone_number AS PhoneNumber, u.is_active AS IsActive,
                   u.created_at AS CreatedAt,
                   COALESCE((SELECT r.name FROM auth.user_roles ur
                             JOIN auth.roles r ON r.id = ur.role_id
                             WHERE ur.user_id = u.id AND ur.is_deleted = FALSE
                             LIMIT 1), '') AS Role
            FROM auth.users u
            INNER JOIN auth.user_companies uc ON uc.user_id = u.id AND uc.is_deleted = FALSE
            WHERE uc.company_id = (
                SELECT company_id FROM auth.user_companies
                WHERE user_id = @DirectorId AND role = 'owner' AND is_deleted = FALSE
                LIMIT 1
            )
            AND u.is_deleted = FALSE
            ORDER BY u.created_at DESC
            """,
            new { DirectorId = directorId });

        return users.AsList();
    }

    public async Task<UserSearchResultDto?> SearchUserAsync(string query, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<UserSearchResultDto>(
            """
            SELECT id, first_name AS FirstName, last_name AS LastName
            FROM auth.users
            WHERE (phone_number = @Query OR email = @Query)
              AND is_deleted = FALSE
            LIMIT 1
            """,
            new { Query = query });
    }

    public async Task<List<CompanyDto>> GetCompaniesForUserAsync(int userId, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var companies = await connection.QueryAsync<CompanyDto>(
            """
            SELECT c.id, c.name, uc.role
            FROM auth.companies c
            INNER JOIN auth.user_companies uc ON uc.company_id = c.id
            WHERE uc.user_id = @UserId AND uc.is_deleted = FALSE AND c.is_deleted = FALSE
            ORDER BY c.name
            """,
            new { UserId = userId });

        return companies.AsList();
    }

    public async Task DeleteUserFromCompanyAsync(int directorId, int userId, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            """
            UPDATE auth.user_companies
            SET is_deleted = TRUE, deleted_at = NOW(), deleted_by = @DirectorId
            WHERE user_id = @UserId
              AND company_id = (
                  SELECT company_id FROM auth.user_companies
                  WHERE user_id = @DirectorId AND role = 'owner' AND is_deleted = FALSE
                  LIMIT 1
              )
              AND is_deleted = FALSE
            """,
            new { DirectorId = directorId, UserId = userId });
    }

    private static string GeneratePassword(int length)
    {
        const string chars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789!@#$";
        return new string(RandomNumberGenerator.GetBytes(length)
            .Select(b => chars[b % chars.Length])
            .ToArray());
    }
}
