using System.Data;
using FluentAssertions;
using GnDapper.Connection;
using Kouroukan.Api.Gateway.Controllers;
using Kouroukan.Api.Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kouroukan.Api.Gateway.Tests.Controllers;

public sealed class GeoControllerTests
{
    private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;
    private readonly IMemoryCache _cache;
    private readonly Mock<ILogger<GeoController>> _loggerMock;
    private readonly GeoController _sut;

    public GeoControllerTests()
    {
        _connectionFactoryMock = new Mock<IDbConnectionFactory>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _loggerMock = new Mock<ILogger<GeoController>>();
        _sut = new GeoController(_connectionFactoryMock.Object, _cache, _loggerMock.Object);
    }

    // ─── GetRegions ───

    [Fact]
    public async Task GetRegions_DoitRetournerRegionsDepuisCache_QuandCacheRempli()
    {
        // Arrange
        var regions = new List<GeoItemDto>
        {
            new("Conakry", "CKY"),
            new("Boke", "BOK")
        };
        _cache.Set("geo:regions", (IEnumerable<GeoItemDto>)regions, TimeSpan.FromHours(24));

        // Act
        var result = await _sut.GetRegions();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<GeoItemDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);

        // Verify no DB call was made
        _connectionFactoryMock.Verify(x => x.CreateConnection(), Times.Never);
    }

    // ─── GetPrefectures ───

    [Fact]
    public async Task GetPrefectures_DoitRetournerBadRequest_QuandRegionCodeVide()
    {
        // Act
        var result = await _sut.GetPrefectures("");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetPrefectures_DoitRetournerBadRequest_QuandRegionCodeEspaces()
    {
        // Act
        var result = await _sut.GetPrefectures("   ");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetPrefectures_DoitRetournerPrefecturesDepuisCache_QuandCacheRempli()
    {
        // Arrange
        var prefectures = new List<GeoItemDto> { new("Boke", "BOK") };
        _cache.Set("geo:prefectures:BOK", (IEnumerable<GeoItemDto>)prefectures, TimeSpan.FromHours(24));

        // Act
        var result = await _sut.GetPrefectures("bok");

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<GeoItemDto>>>().Subject;
        response.Data.Should().HaveCount(1);
        _connectionFactoryMock.Verify(x => x.CreateConnection(), Times.Never);
    }

    // ─── GetSousPrefectures ───

    [Fact]
    public async Task GetSousPrefectures_DoitRetournerBadRequest_QuandPrefectureCodeVide()
    {
        // Act
        var result = await _sut.GetSousPrefectures("");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetSousPrefectures_DoitRetournerBadRequest_QuandPrefectureCodeEspaces()
    {
        // Act
        var result = await _sut.GetSousPrefectures("   ");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetSousPrefectures_DoitRetournerSousPrefecturesDepuisCache_QuandCacheRempli()
    {
        // Arrange
        var sousPrefectures = new List<GeoItemDto> { new("Kankan Centre", "KNKC") };
        _cache.Set("geo:sous-prefectures:KNK", (IEnumerable<GeoItemDto>)sousPrefectures, TimeSpan.FromHours(24));

        // Act
        var result = await _sut.GetSousPrefectures("knk");

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<GeoItemDto>>>().Subject;
        response.Data.Should().HaveCount(1);
        _connectionFactoryMock.Verify(x => x.CreateConnection(), Times.Never);
    }
}
