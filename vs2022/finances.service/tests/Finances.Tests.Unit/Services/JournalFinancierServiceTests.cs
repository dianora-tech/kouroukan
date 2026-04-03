using Finances.Domain.Entities;
using Finances.Domain.Ports.Output;
using Finances.Domain.Services;
using FluentAssertions;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Finances.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour JournalFinancierService.
/// </summary>
public sealed class JournalFinancierServiceTests
{
    private readonly Mock<IJournalFinancierRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<JournalFinancierService>> _loggerMock;
    private readonly JournalFinancierService _sut;

    public JournalFinancierServiceTests()
    {
        _repoMock = new Mock<IJournalFinancierRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<JournalFinancierService>>();

        _sut = new JournalFinancierService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneEntree_QuandExiste()
    {
        var entity = CreateEntree();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_CreeLEntree_AvecDonneesValides()
    {
        var entity = CreateEntree();
        _repoMock.Setup(r => r.AddAsync(It.IsAny<JournalFinancier>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((JournalFinancier e, CancellationToken _) => e);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<JournalFinancier>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_PublieEvenement_ApresCreation()
    {
        var entity = CreateEntree();
        _repoMock.Setup(r => r.AddAsync(It.IsAny<JournalFinancier>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((JournalFinancier e, CancellationToken _) => e);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<JournalFinancier>>(),
            "kouroukan.events",
            "entity.created.journal-financier",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiMontantZeroOuNegatif()
    {
        var entity = CreateEntree();
        entity.Montant = 0m;

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*montant doit etre superieur a 0*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiTypeVide()
    {
        var entity = CreateEntree();
        entity.Type = "";

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*type d'operation est obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_DefinitDateOperationParDefaut_SiDefault()
    {
        var entity = CreateEntree();
        entity.DateOperation = default;

        _repoMock.Setup(r => r.AddAsync(It.IsAny<JournalFinancier>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((JournalFinancier e, CancellationToken _) => e);

        var result = await _sut.CreateAsync(entity);

        result.DateOperation.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    // --- Helper ---

    private static JournalFinancier CreateEntree()
    {
        return new JournalFinancier
        {
            Id = 1,
            CompanyId = 1,
            Type = "Recette",
            Categorie = "Scolarite",
            Montant = 150000m,
            Description = "Paiement scolarite",
            ModePaiement = "OrangeMoney",
            DateOperation = new DateTime(2025, 9, 1)
        };
    }
}
