using Finances.Domain.Entities;
using Finances.Domain.Ports.Output;
using Finances.Domain.Services;
using FluentAssertions;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Finances.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour PaiementService.
/// </summary>
public sealed class PaiementServiceTests
{
    private readonly Mock<IPaiementRepository> _repoMock;
    private readonly Mock<IFactureRepository> _factureRepoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<PaiementService>> _loggerMock;
    private readonly PaiementService _sut;

    public PaiementServiceTests()
    {
        _repoMock = new Mock<IPaiementRepository>();
        _factureRepoMock = new Mock<IFactureRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<PaiementService>>();

        _sut = new PaiementService(
            _repoMock.Object,
            _factureRepoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_RetournePaiement_QuandExiste()
    {
        var entity = CreatePaiement();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistant()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Paiement?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_CreeLePaiement_AvecDonneesValides()
    {
        var entity = CreatePaiement();
        _repoMock.Setup(r => r.GetByNumeroRecuAsync(entity.NumeroRecu, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Paiement?)null);
        _factureRepoMock.Setup(r => r.GetByIdAsync(entity.FactureId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Facture { Id = entity.FactureId });
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
        _repoMock.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_PublieEvenement_ApresCreation()
    {
        var entity = CreatePaiement();
        _repoMock.Setup(r => r.GetByNumeroRecuAsync(entity.NumeroRecu, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Paiement?)null);
        _factureRepoMock.Setup(r => r.GetByIdAsync(entity.FactureId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Facture { Id = entity.FactureId });
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Paiement>>(),
            "kouroukan.events",
            "entity.created.paiement",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiNumeroRecuDuplique()
    {
        var entity = CreatePaiement();
        _repoMock.Setup(r => r.GetByNumeroRecuAsync(entity.NumeroRecu, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*numero de recu*existe deja*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiMoyenPaiementInvalide()
    {
        var entity = CreatePaiement(moyenPaiement: "Bitcoin");
        _repoMock.Setup(r => r.GetByNumeroRecuAsync(entity.NumeroRecu, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Paiement?)null);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*moyen de paiement*n'est pas autorise*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiMontantZeroOuNegatif()
    {
        var entity = CreatePaiement(montant: 0m);
        _repoMock.Setup(r => r.GetByNumeroRecuAsync(entity.NumeroRecu, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Paiement?)null);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*montant paye doit etre superieur a zero*");
    }

    [Fact]
    public async Task CreateAsync_Lance_KeyNotFoundException_SiFactureInexistante()
    {
        var entity = CreatePaiement();
        _repoMock.Setup(r => r.GetByNumeroRecuAsync(entity.NumeroRecu, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Paiement?)null);
        _factureRepoMock.Setup(r => r.GetByIdAsync(entity.FactureId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Facture?)null);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*facture*introuvable*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiReferenceMobileMoneyManquantePourPaiementElectronique()
    {
        var entity = CreatePaiement(moyenPaiement: "OrangeMoney");
        entity.ReferenceMobileMoney = null;

        _repoMock.Setup(r => r.GetByNumeroRecuAsync(entity.NumeroRecu, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Paiement?)null);
        _factureRepoMock.Setup(r => r.GetByIdAsync(entity.FactureId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Facture { Id = entity.FactureId });

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*reference Mobile Money est obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_PasDerreurReferenceMobileMoney_PourEspeces()
    {
        var entity = CreatePaiement(moyenPaiement: "Especes");
        entity.ReferenceMobileMoney = null;

        _repoMock.Setup(r => r.GetByNumeroRecuAsync(entity.NumeroRecu, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Paiement?)null);
        _factureRepoMock.Setup(r => r.GetByIdAsync(entity.FactureId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Facture { Id = entity.FactureId });
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData("OrangeMoney")]
    [InlineData("SoutraMoney")]
    [InlineData("MTNMoMo")]
    [InlineData("Especes")]
    public async Task CreateAsync_AccepteTousLesMoyensPaiementAutorises(string moyen)
    {
        var entity = CreatePaiement(moyenPaiement: moyen);
        if (moyen != "Especes")
            entity.ReferenceMobileMoney = "REF-12345";

        _repoMock.Setup(r => r.GetByNumeroRecuAsync(entity.NumeroRecu, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Paiement?)null);
        _factureRepoMock.Setup(r => r.GetByIdAsync(entity.FactureId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Facture { Id = entity.FactureId });
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreatePaiement();
        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiMoyenPaiementInvalide()
    {
        var entity = CreatePaiement(moyenPaiement: "Bitcoin");

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*moyen de paiement*n'est pas autorise*");
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiMontantZero()
    {
        var entity = CreatePaiement(montant: 0m);

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*montant paye doit etre superieur a zero*");
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_RetourneTrue_QuandSuppressionReussie()
    {
        _repoMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.DeleteAsync(1);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_RetourneFalse_QuandInexistant()
    {
        _repoMock.Setup(r => r.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.DeleteAsync(999);

        result.Should().BeFalse();
    }

    // --- Helper ---

    private static Paiement CreatePaiement(
        decimal montant = 50000m,
        string moyenPaiement = "Especes")
    {
        return new Paiement
        {
            Id = 1,
            TypeId = 1,
            FactureId = 10,
            MontantPaye = montant,
            DatePaiement = new DateTime(2025, 9, 15),
            MoyenPaiement = moyenPaiement,
            ReferenceMobileMoney = moyenPaiement != "Especes" ? "REF-12345" : null,
            StatutPaiement = "Confirme",
            CaissierId = 3,
            NumeroRecu = "REC-2025-001",
            UserId = 1
        };
    }
}
