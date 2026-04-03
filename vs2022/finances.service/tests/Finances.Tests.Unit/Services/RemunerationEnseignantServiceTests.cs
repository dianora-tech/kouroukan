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
/// Tests unitaires pour RemunerationEnseignantService.
/// </summary>
public sealed class RemunerationEnseignantServiceTests
{
    private readonly Mock<IRemunerationEnseignantRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<RemunerationEnseignantService>> _loggerMock;
    private readonly RemunerationEnseignantService _sut;

    public RemunerationEnseignantServiceTests()
    {
        _repoMock = new Mock<IRemunerationEnseignantRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<RemunerationEnseignantService>>();

        _sut = new RemunerationEnseignantService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_RetourneRemuneration_QuandExiste()
    {
        var entity = CreateRemuneration();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_CreeLaRemuneration_ModeForfait()
    {
        var entity = CreateRemuneration(mode: "Forfait", montantForfait: 2000000m);
        _repoMock.Setup(r => r.GetByEnseignantMoisAnneeAsync(entity.EnseignantId, entity.Mois, entity.Annee, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RemunerationEnseignant?)null);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<RemunerationEnseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RemunerationEnseignant e, CancellationToken _) => e);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
        result.MontantTotal.Should().Be(2000000m);
    }

    [Fact]
    public async Task CreateAsync_CreeLaRemuneration_ModeHeures()
    {
        var entity = CreateRemuneration(mode: "Heures", nombreHeures: 40m, tauxHoraire: 50000m);
        _repoMock.Setup(r => r.GetByEnseignantMoisAnneeAsync(entity.EnseignantId, entity.Mois, entity.Annee, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RemunerationEnseignant?)null);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<RemunerationEnseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RemunerationEnseignant e, CancellationToken _) => e);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
        result.MontantTotal.Should().Be(2000000m); // 40 * 50000
    }

    [Fact]
    public async Task CreateAsync_CreeLaRemuneration_ModeMixte()
    {
        var entity = CreateRemuneration(mode: "Mixte", montantForfait: 1000000m, nombreHeures: 10m, tauxHoraire: 50000m);
        _repoMock.Setup(r => r.GetByEnseignantMoisAnneeAsync(entity.EnseignantId, entity.Mois, entity.Annee, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RemunerationEnseignant?)null);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<RemunerationEnseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RemunerationEnseignant e, CancellationToken _) => e);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
        result.MontantTotal.Should().Be(1500000m); // 1000000 + 10 * 50000
    }

    [Fact]
    public async Task CreateAsync_PublieEvenement_ApresCreation()
    {
        var entity = CreateRemuneration();
        _repoMock.Setup(r => r.GetByEnseignantMoisAnneeAsync(entity.EnseignantId, entity.Mois, entity.Annee, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RemunerationEnseignant?)null);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<RemunerationEnseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RemunerationEnseignant e, CancellationToken _) => e);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<RemunerationEnseignant>>(),
            "kouroukan.events",
            "entity.created.remunerationenseignant",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiDoublonMoisAnnee()
    {
        var entity = CreateRemuneration();
        _repoMock.Setup(r => r.GetByEnseignantMoisAnneeAsync(entity.EnseignantId, entity.Mois, entity.Annee, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*remuneration existe deja*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiModeInvalide()
    {
        var entity = CreateRemuneration(mode: "INVALIDE");

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*mode de remuneration*n'est pas autorise*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiStatutInvalide()
    {
        var entity = CreateRemuneration(statut: "INVALIDE");

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*statut de paiement*n'est pas autorise*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiMoisInvalide()
    {
        var entity = CreateRemuneration();
        entity.Mois = 13;

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*mois doit etre compris entre 1 et 12*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiAnneeInvalide()
    {
        var entity = CreateRemuneration();
        entity.Annee = 1999;

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*annee doit etre superieure ou egale a 2000*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiForfaitManquantPourModeForfait()
    {
        var entity = CreateRemuneration(mode: "Forfait");
        entity.MontantForfait = null;

        _repoMock.Setup(r => r.GetByEnseignantMoisAnneeAsync(entity.EnseignantId, entity.Mois, entity.Annee, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RemunerationEnseignant?)null);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*montant forfait est obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiHeuresManquantesPourModeHeures()
    {
        var entity = CreateRemuneration(mode: "Heures", nombreHeures: null, tauxHoraire: 50000m);

        _repoMock.Setup(r => r.GetByEnseignantMoisAnneeAsync(entity.EnseignantId, entity.Mois, entity.Annee, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RemunerationEnseignant?)null);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*nombre d'heures est obligatoire*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiTauxHoraireManquantPourModeHeures()
    {
        var entity = CreateRemuneration(mode: "Heures", nombreHeures: 40m, tauxHoraire: null);

        _repoMock.Setup(r => r.GetByEnseignantMoisAnneeAsync(entity.EnseignantId, entity.Mois, entity.Annee, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RemunerationEnseignant?)null);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*taux horaire est obligatoire*");
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateRemuneration();
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<RemunerationEnseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_RecalculeMontantTotal()
    {
        var entity = CreateRemuneration(mode: "Heures", nombreHeures: 20m, tauxHoraire: 100000m);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<RemunerationEnseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        entity.MontantTotal.Should().Be(2000000m);
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
    public async Task DeleteAsync_RetourneFalse_QuandInexistante()
    {
        _repoMock.Setup(r => r.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.DeleteAsync(999);

        result.Should().BeFalse();
    }

    // --- Helper ---

    private static RemunerationEnseignant CreateRemuneration(
        string mode = "Forfait",
        string statut = "EnAttente",
        decimal? montantForfait = 2000000m,
        decimal? nombreHeures = null,
        decimal? tauxHoraire = null)
    {
        return new RemunerationEnseignant
        {
            Id = 1,
            EnseignantId = 10,
            Mois = 9,
            Annee = 2025,
            ModeRemuneration = mode,
            MontantForfait = montantForfait,
            NombreHeures = nombreHeures,
            TauxHoraire = tauxHoraire,
            MontantTotal = 0m,
            StatutPaiement = statut,
            UserId = 1
        };
    }
}
