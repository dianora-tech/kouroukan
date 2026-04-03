using FluentAssertions;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.Models;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Output;
using Inscriptions.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Inscriptions.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour RadiationService.
/// </summary>
public sealed class RadiationServiceTests
{
    private readonly Mock<IRadiationRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<RadiationService>> _loggerMock;
    private readonly RadiationService _sut;

    public RadiationServiceTests()
    {
        _repoMock = new Mock<IRadiationRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<RadiationService>>();

        _sut = new RadiationService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneRadiation_QuandExiste()
    {
        var radiation = CreateRadiation();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(radiation);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistante()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Radiation?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<Radiation>(
            new List<Radiation> { CreateRadiation() }, 1, 1, 20);
        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null);

        result.Items.Should().HaveCount(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeLaRadiation_QuandValide()
    {
        var entity = CreateRadiation();

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Radiation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        _repoMock.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DefinitDateRadiation()
    {
        var entity = CreateRadiation();
        entity.DateRadiation = default;

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Radiation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        entity.DateRadiation.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task CreateAsync_PublieEvenement_ApresCreation()
    {
        var entity = CreateRadiation();

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Radiation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Radiation>>(),
            "kouroukan.events",
            "entity.created.radiation",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiMotifVide()
    {
        var entity = CreateRadiation();
        entity.Motif = "";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*motif*obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiMotifNull()
    {
        var entity = CreateRadiation();
        entity.Motif = null!;

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*motif*obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiMotifEspacesBlancs()
    {
        var entity = CreateRadiation();
        entity.Motif = "   ";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*motif*obligatoire*");
    }

    // ─── Helper ───

    private static Radiation CreateRadiation()
    {
        return new Radiation
        {
            Id = 1,
            EleveId = 10,
            CompanyId = 1,
            Motif = "Absenteisme repete",
            DateRadiation = DateTime.UtcNow
        };
    }
}
