using Dapper;
using GnDapper.Connection;
using GnSecurity.Hashing;
using GnSecurity.Jwt;
using Kouroukan.Api.Gateway.Models;
using ConflictException = Kouroukan.Api.Gateway.Models.ConflictException;

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

        var companies = (await connection.QueryAsync<CompanyDto>(
            """
            SELECT c.id, c.name, uc.role
            FROM auth.companies c
            INNER JOIN auth.user_companies uc ON uc.company_id = c.id
            WHERE uc.user_id = @UserId AND uc.is_deleted = FALSE AND c.is_deleted = FALSE
            ORDER BY c.name
            """,
            new { UserId = userId })).AsList();

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
            CguAcceptedAt = user.CguAcceptedAt,
            Companies = companies
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

    /// <inheritdoc />
    public async Task<AuthTokensDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        // ── 1. Verifier l'unicite du numero de telephone ──────────────────────
        var phoneExists = await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM auth.users WHERE phone_number = @Phone AND is_deleted = FALSE)",
            new { Phone = request.PhoneNumber });

        if (phoneExists)
            throw new ConflictException("Un compte avec ce numero de telephone existe deja.");

        // ── 2. Verifier l'unicite de l'email (si fourni) ──────────────────────
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailExists = await connection.ExecuteScalarAsync<bool>(
                "SELECT EXISTS(SELECT 1 FROM auth.users WHERE email = @Email AND is_deleted = FALSE)",
                new { Email = request.Email });

            if (emailExists)
                throw new ConflictException("Un compte avec cet email existe deja.");
        }

        // ── 3. Hasher le mot de passe ─────────────────────────────────────────
        var passwordHash = await _passwordHasher.HashAsync(request.Password, cancellationToken);

        // Email : utiliser le vrai email ou generer un placeholder unique a partir du telephone
        var effectiveEmail = !string.IsNullOrWhiteSpace(request.Email)
            ? request.Email
            : $"{request.PhoneNumber.Replace("+", "").Replace(" ", "")}@kouroukan.gn";

        // ── 4. Creer l'utilisateur (directeur) ───────────────────────────────
        var userId = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO auth.users (first_name, last_name, email, phone_number, password_hash, is_active)
            VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @PasswordHash, TRUE)
            RETURNING id
            """,
            new
            {
                request.FirstName,
                request.LastName,
                Email = effectiveEmail,
                request.PhoneNumber,
                PasswordHash = passwordHash
            });

        // ── 5. Assigner le role 'directeur' ───────────────────────────────────
        await connection.ExecuteAsync(
            """
            INSERT INTO auth.user_roles (user_id, role_id)
            SELECT @UserId, id FROM auth.roles WHERE name = 'directeur' AND is_deleted = FALSE
            """,
            new { UserId = userId });

        // ── 6. Creer l'etablissement (company / tenant) ───────────────────────
        var schoolName = string.IsNullOrWhiteSpace(request.SchoolName)
            ? $"{request.FirstName} {request.LastName}"
            : request.SchoolName;

        var companyId = await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO auth.companies (name, phone_number, email, address, modules)
            VALUES (@Name, @PhoneNumber, @Email, @Address, @Modules::text[])
            RETURNING id
            """,
            new
            {
                Name = schoolName,
                PhoneNumber = request.PhoneNumber,
                Email = string.IsNullOrWhiteSpace(request.Email) ? (string?)null : request.Email,
                Address = request.Address,
                Modules = request.Modules.ToArray()
            });

        // ── 7. Lier l'utilisateur a la company (role owner) ───────────────────
        await connection.ExecuteAsync(
            """
            INSERT INTO auth.user_companies (user_id, company_id, role)
            VALUES (@UserId, @CompanyId, 'owner')
            """,
            new { UserId = userId, CompanyId = companyId });

        // ── 8. Enregistrer la localisation si sous-prefecture fournie ─────────
        if (!string.IsNullOrWhiteSpace(request.SousPrefecture))
        {
            var sousPrefectureId = await connection.ExecuteScalarAsync<int?>(
                """
                SELECT sp.id
                FROM geo.sous_prefectures sp
                INNER JOIN geo.prefectures p ON p.id = sp.prefecture_id
                WHERE sp.code = @Code AND sp.is_deleted = FALSE
                LIMIT 1
                """,
                new { Code = request.SousPrefecture.ToUpperInvariant() });

            if (sousPrefectureId.HasValue)
            {
                await connection.ExecuteAsync(
                    """
                    INSERT INTO geo.user_locations (user_id, sous_prefecture_id, address)
                    VALUES (@UserId, @SousPrefectureId, @Address)
                    """,
                    new { UserId = userId, SousPrefectureId = sousPrefectureId.Value, request.Address });
            }
        }

        // ── 9. Charger roles et permissions, generer les tokens ───────────────
        var roles = await GetRolesForUserAsync(connection, userId);
        var permissions = await GetPermissionsForUserAsync(connection, userId);

        var tokenResult = _jwtTokenService.GenerateTokens(
            userId,
            effectiveEmail,
            $"{request.FirstName} {request.LastName}",
            roles,
            permissions);

        await _refreshTokenStore.StoreAsync(
            userId,
            tokenResult.RefreshToken,
            tokenResult.RefreshTokenExpiresAt,
            cancellationToken);

        _logger.LogInformation(
            "Inscription reussie pour le directeur {UserId} ({Phone}), etablissement {CompanyId} '{SchoolName}'",
            userId, request.PhoneNumber, companyId, schoolName);

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
