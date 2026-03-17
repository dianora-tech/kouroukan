using Dapper;
using GnDapper.Connection;
using GnSecurity.Hashing;
using GnSecurity.Jwt;
using Kouroukan.Api.Gateway.Models;

namespace Kouroukan.Api.Gateway.Auth;

/// <summary>
/// Implementation du service de tokens d'authentification.
/// </summary>
public sealed class TokenService : ITokenService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly ILogger<TokenService> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="TokenService"/>.
    /// </summary>
    public TokenService(
        IDbConnectionFactory connectionFactory,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IRefreshTokenStore refreshTokenStore,
        ILogger<TokenService> logger)
    {
        _connectionFactory = connectionFactory;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _refreshTokenStore = refreshTokenStore;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AuthTokensDto> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var user = await connection.QuerySingleOrDefaultAsync<AuthUser>(
            """
            SELECT id, first_name, last_name, email, phone_number, password_hash,
                   is_active, last_login_at, cgu_accepted_at, cgu_version, is_deleted
            FROM auth.users
            WHERE email = @Email AND is_deleted = FALSE
            """,
            new { Email = email });

        if (user is null)
        {
            _logger.LogWarning("Tentative de connexion avec un email inconnu : {Email}", email);
            throw new UnauthorizedAccessException("Email ou mot de passe incorrect.");
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Tentative de connexion sur un compte inactif : {UserId}", user.Id);
            throw new UnauthorizedAccessException("Ce compte est desactive.");
        }

        var isPasswordValid = await _passwordHasher.VerifyAsync(password, user.PasswordHash, cancellationToken);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Mot de passe incorrect pour l'utilisateur {UserId}", user.Id);
            throw new UnauthorizedAccessException("Email ou mot de passe incorrect.");
        }

        // Charger roles et permissions
        var roles = await GetRolesForUserAsync(connection, user.Id);
        var permissions = await GetPermissionsForUserAsync(connection, user.Id);

        // Generer les tokens (inclure cguVersion comme claim custom)
        var tokenResult = _jwtTokenService.GenerateTokens(
            user.Id,
            user.Email,
            $"{user.FirstName} {user.LastName}",
            roles,
            permissions);

        // Stocker le refresh token
        await _refreshTokenStore.StoreAsync(
            user.Id,
            tokenResult.RefreshToken,
            tokenResult.RefreshTokenExpiresAt,
            cancellationToken);

        // Mettre a jour last_login_at
        await connection.ExecuteAsync(
            "UPDATE auth.users SET last_login_at = NOW() WHERE id = @Id",
            new { user.Id });

        _logger.LogInformation("Connexion reussie pour l'utilisateur {UserId} ({Email})", user.Id, user.Email);

        return new AuthTokensDto
        {
            AccessToken = tokenResult.AccessToken,
            RefreshToken = tokenResult.RefreshToken,
            AccessTokenExpiresAt = tokenResult.AccessTokenExpiresAt,
            RefreshTokenExpiresAt = tokenResult.RefreshTokenExpiresAt
        };
    }

    /// <inheritdoc />
    public async Task<UserProfileDto?> GetUserProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        var user = await connection.QuerySingleOrDefaultAsync<AuthUser>(
            """
            SELECT id, first_name, last_name, email, phone_number,
                   is_active, cgu_accepted_at, cgu_version
            FROM auth.users
            WHERE id = @UserId AND is_deleted = FALSE
            """,
            new { UserId = userId });

        if (user is null) return null;

        var roles = await GetRolesForUserAsync(connection, userId);
        var permissions = await GetPermissionsForUserAsync(connection, userId);

        return new UserProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Roles = roles,
            Permissions = permissions,
            CguVersion = user.CguVersion,
            CguAcceptedAt = user.CguAcceptedAt
        };
    }

    /// <inheritdoc />
    public async Task<CguVersionDto?> GetActiveCguAsync(CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<CguVersionDto>(
            """
            SELECT version, contenu, date_publication
            FROM auth.cgu_versions
            WHERE est_active = TRUE AND is_deleted = FALSE
            LIMIT 1
            """);
    }

    /// <inheritdoc />
    public async Task<AuthTokensDto> AcceptCguAsync(int userId, string cguVersion, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            """
            UPDATE auth.users
            SET cgu_accepted_at = NOW(), cgu_version = @CguVersion, updated_at = NOW()
            WHERE id = @UserId
            """,
            new { UserId = userId, CguVersion = cguVersion });

        var user = await connection.QuerySingleOrDefaultAsync<AuthUser>(
            """
            SELECT id, first_name, last_name, email, phone_number,
                   cgu_accepted_at, cgu_version
            FROM auth.users
            WHERE id = @UserId AND is_deleted = FALSE
            """,
            new { UserId = userId });

        if (user is null)
            throw new InvalidOperationException("Utilisateur introuvable.");

        var roles = await GetRolesForUserAsync(connection, userId);
        var permissions = await GetPermissionsForUserAsync(connection, userId);

        var tokenResult = _jwtTokenService.GenerateTokens(
            user.Id,
            user.Email,
            $"{user.FirstName} {user.LastName}",
            roles,
            permissions);

        await _refreshTokenStore.StoreAsync(
            user.Id,
            tokenResult.RefreshToken,
            tokenResult.RefreshTokenExpiresAt,
            cancellationToken);

        _logger.LogInformation("CGU version {CguVersion} acceptee par l'utilisateur {UserId}", cguVersion, userId);

        return new AuthTokensDto
        {
            AccessToken = tokenResult.AccessToken,
            RefreshToken = tokenResult.RefreshToken,
            AccessTokenExpiresAt = tokenResult.AccessTokenExpiresAt,
            RefreshTokenExpiresAt = tokenResult.RefreshTokenExpiresAt
        };
    }

    private static async Task<List<string>> GetRolesForUserAsync(System.Data.IDbConnection connection, int userId)
    {
        var roles = await connection.QueryAsync<string>(
            """
            SELECT r.name
            FROM auth.user_roles ur
            INNER JOIN auth.roles r ON r.id = ur.role_id
            WHERE ur.user_id = @UserId
              AND ur.is_deleted = FALSE
              AND r.is_deleted = FALSE
            """,
            new { UserId = userId });

        return roles.ToList();
    }

    private static async Task<List<string>> GetPermissionsForUserAsync(System.Data.IDbConnection connection, int userId)
    {
        var permissions = await connection.QueryAsync<string>(
            """
            SELECT DISTINCT p.name
            FROM auth.user_roles ur
            INNER JOIN auth.role_permissions rp ON rp.role_id = ur.role_id
            INNER JOIN auth.permissions p ON p.id = rp.permission_id
            WHERE ur.user_id = @UserId
              AND ur.is_deleted = FALSE
              AND rp.is_deleted = FALSE
              AND p.is_deleted = FALSE
            """,
            new { UserId = userId });

        return permissions.ToList();
    }
}
