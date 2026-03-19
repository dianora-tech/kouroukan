using Dapper;
using GnDapper.Connection;
using Kouroukan.Api.Gateway.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Kouroukan.Api.Gateway.Controllers;

/// <summary>
/// Controleur de donnees geographiques (regions, prefectures, sous-prefectures).
/// Les endpoints sont publics et mis en cache memoire pour limiter les requetes DB.
/// </summary>
[ApiController]
[Route("api/geo")]
[AllowAnonymous]
public sealed class GeoController : ControllerBase
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GeoController> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="GeoController"/>.
    /// </summary>
    public GeoController(
        IDbConnectionFactory connectionFactory,
        IMemoryCache cache,
        ILogger<GeoController> logger)
    {
        _connectionFactory = connectionFactory;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Retourne la liste des 8 regions administratives de Guinee.
    /// </summary>
    [HttpGet("regions")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<GeoItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRegions()
    {
        const string cacheKey = "geo:regions";

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<GeoItemDto>? regions))
        {
            using var connection = _connectionFactory.CreateConnection();
            regions = await connection.QueryAsync<GeoItemDto>(
                """
                SELECT name, code
                FROM geo.regions
                WHERE is_deleted = FALSE
                ORDER BY name
                """);

            _cache.Set(cacheKey, regions, CacheDuration);
            _logger.LogDebug("geo:regions rechargees depuis la base de donnees");
        }

        return Ok(ApiResponse<IEnumerable<GeoItemDto>>.Ok(regions!));
    }

    /// <summary>
    /// Retourne les prefectures d'une region donnee (filtrage par code region).
    /// Pour Conakry (CKY), retourne les 5 communes.
    /// </summary>
    /// <param name="regionCode">Code de la region (ex: CKY, BOK, ...).</param>
    [HttpGet("prefectures")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<GeoItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPrefectures([FromQuery] string regionCode)
    {
        if (string.IsNullOrWhiteSpace(regionCode))
            return BadRequest(ApiResponse<object>.Fail("Le parametre regionCode est requis."));

        var cacheKey = $"geo:prefectures:{regionCode.ToUpperInvariant()}";

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<GeoItemDto>? prefectures))
        {
            using var connection = _connectionFactory.CreateConnection();
            prefectures = await connection.QueryAsync<GeoItemDto>(
                """
                SELECT p.name, p.code
                FROM geo.prefectures p
                INNER JOIN geo.regions r ON r.id = p.region_id
                WHERE r.code = @RegionCode
                  AND p.is_deleted = FALSE
                  AND r.is_deleted = FALSE
                ORDER BY p.name
                """,
                new { RegionCode = regionCode.ToUpperInvariant() });

            _cache.Set(cacheKey, prefectures, CacheDuration);
        }

        return Ok(ApiResponse<IEnumerable<GeoItemDto>>.Ok(prefectures!));
    }

    /// <summary>
    /// Retourne les sous-prefectures d'une prefecture donnee (filtrage par code prefecture).
    /// Conakry n'a pas de sous-prefectures (retourne une liste vide).
    /// </summary>
    /// <param name="prefectureCode">Code de la prefecture (ex: KLM, BOK, ...).</param>
    [HttpGet("sous-prefectures")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<GeoItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSousPrefectures([FromQuery] string prefectureCode)
    {
        if (string.IsNullOrWhiteSpace(prefectureCode))
            return BadRequest(ApiResponse<object>.Fail("Le parametre prefectureCode est requis."));

        var cacheKey = $"geo:sous-prefectures:{prefectureCode.ToUpperInvariant()}";

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<GeoItemDto>? sousPrefectures))
        {
            using var connection = _connectionFactory.CreateConnection();
            sousPrefectures = await connection.QueryAsync<GeoItemDto>(
                """
                SELECT sp.name, sp.code
                FROM geo.sous_prefectures sp
                INNER JOIN geo.prefectures p ON p.id = sp.prefecture_id
                WHERE p.code = @PrefectureCode
                  AND sp.is_deleted = FALSE
                  AND p.is_deleted = FALSE
                ORDER BY sp.name
                """,
                new { PrefectureCode = prefectureCode.ToUpperInvariant() });

            _cache.Set(cacheKey, sousPrefectures, CacheDuration);
        }

        return Ok(ApiResponse<IEnumerable<GeoItemDto>>.Ok(sousPrefectures!));
    }
}

/// <summary>
/// DTO generique pour un element geographique (region, prefecture, sous-prefecture).
/// </summary>
public record GeoItemDto(string Name, string Code);
