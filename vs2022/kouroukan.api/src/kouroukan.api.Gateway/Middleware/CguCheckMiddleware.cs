using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Kouroukan.Api.Gateway.Auth;

namespace Kouroukan.Api.Gateway.Middleware;

/// <summary>
/// Middleware qui verifie si l'utilisateur authentifie a accepte la version active des CGU.
/// S'execute apres Authentication et avant Authorization.
/// Retourne 403 si les CGU ne sont pas acceptees (sauf routes exclues).
/// </summary>
public sealed class CguCheckMiddleware
{
    private const string CguCacheKey = "cgu:active_version";
    private readonly RequestDelegate _next;
    private readonly ILogger<CguCheckMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>Routes exclues du check CGU.</summary>
    private static readonly string[] ExcludedPaths =
    [
        "/api/auth/login",
        "/api/auth/refresh",
        "/api/auth/logout",
        "/api/auth/me",
        "/api/auth/cgu",
        "/health",
        "/ready"
    ];

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="CguCheckMiddleware"/>.
    /// </summary>
    public CguCheckMiddleware(RequestDelegate next, ILogger<CguCheckMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Verifie l'acceptation des CGU pour les requetes authentifiees.
    /// </summary>
    public async Task InvokeAsync(HttpContext context, IDistributedCache cache, ITokenService tokenService)
    {
        // Passer si l'utilisateur n'est pas authentifie
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        // Passer si route exclue
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
        if (IsExcludedPath(path))
        {
            await _next(context);
            return;
        }

        // Recuperer la version active des CGU (cache Redis, TTL 1h)
        var activeCguVersion = await GetActiveCguVersionAsync(cache, tokenService);
        if (string.IsNullOrEmpty(activeCguVersion))
        {
            // Pas de CGU configuree → laisser passer
            await _next(context);
            return;
        }

        // Recuperer la version CGU du token utilisateur
        var userCguVersion = context.User.FindFirst("cguVersion")?.Value;

        if (userCguVersion != activeCguVersion)
        {
            _logger.LogWarning(
                "CGU non acceptees pour l'utilisateur {UserId}. Version token: {UserVersion}, Version active: {ActiveVersion}",
                context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? context.User.FindFirst("sub")?.Value,
                userCguVersion ?? "(aucune)",
                activeCguVersion);

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = new
            {
                code = "CGU_NOT_ACCEPTED",
                message = "Veuillez accepter les nouvelles Conditions Generales d'Utilisation.",
                redirectTo = "/support/cgu"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
            return;
        }

        await _next(context);
    }

    private static bool IsExcludedPath(string path)
    {
        foreach (var excluded in ExcludedPaths)
        {
            if (path.StartsWith(excluded, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    private static async Task<string?> GetActiveCguVersionAsync(IDistributedCache cache, ITokenService tokenService)
    {
        var cached = await cache.GetStringAsync(CguCacheKey);
        if (!string.IsNullOrEmpty(cached))
            return cached;

        var activeCgu = await tokenService.GetActiveCguAsync();
        if (activeCgu is null)
            return null;

        await cache.SetStringAsync(CguCacheKey, activeCgu.Version, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        });

        return activeCgu.Version;
    }
}
