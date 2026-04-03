using FluentAssertions;
using GnDapper.Models;
using GnMessaging.Abstractions;
using GnMessaging.Events;
using GnMessaging.Models;
using Personnel.Domain.Entities;
using Personnel.Domain.Ports.Output;
using Personnel.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Personnel.Tests.Unit.Services;

/// <summary>
/// Tests unitaires pour EnseignantService.
/// </summary>
public sealed class EnseignantServiceTests
{
    private readonly Mock<IEnseignantRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<EnseignantService>> _loggerMock;
    private readonly EnseignantService _sut;

    public EnseignantServiceTests()
    {
        _repoMock = new Mock<IEnseignantRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<EnseignantService>>();

        _sut = new EnseignantService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneEnseignant_QuandExiste()
    {
        var enseignant = CreateEnseignant();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enseignant);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistant()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Enseignant?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetAllAsync ───

    [Fact]
    public async Task GetAllAsync_RetourneListe()
    {
        var enseignants = new List<Enseignant>
        {
            CreateEnseignant(id: 1),
            CreateEnseignant(id: 2, matricule: "MAT-002")
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(enseignants);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<Enseignant>(
            new List<Enseignant> { CreateEnseignant() }, 1, 1, 20);

        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null, null);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeEnseignant_AvecModeRemunerationValide()
    {
        var entity = CreateEnseignant(modeRemuneration: "Forfait");

        _repoMock.Setup(r => r.GetByMatriculeAsync(entity.Matricule, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Enseignant?)null);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        _repoMock.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_PublieEvenement_ApresCreation()
    {
        var entity = CreateEnseignant();

        _repoMock.Setup(r => r.GetByMatriculeAsync(entity.Matricule, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Enseignant?)null);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Enseignant>>(),
            "kouroukan.events",
            "entity.created.enseignant",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiMatriculeExisteDeja()
    {
        var entity = CreateEnseignant();
        var existing = CreateEnseignant(id: 99);

        _repoMock.Setup(r => r.GetByMatriculeAsync(entity.Matricule, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*matricule*existe deja*");
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiModeRemunerationInvalide()
    {
        var entity = CreateEnseignant(modeRemuneration: "INVALIDE");

        _repoMock.Setup(r => r.GetByMatriculeAsync(entity.Matricule, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Enseignant?)null);

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*mode de remuneration*");
    }

    [Theory]
    [InlineData("Forfait")]
    [InlineData("Heures")]
    [InlineData("Mixte")]
    public async Task CreateAsync_AccepteTousLesModesRemunerationValides(string mode)
    {
        var entity = CreateEnseignant(modeRemuneration: mode);

        _repoMock.Setup(r => r.GetByMatriculeAsync(entity.Matricule, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Enseignant?)null);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
    }

    // ─── UpdateAsync ───

    [Fact]
    public async Task UpdateAsync_RetourneTrue_QuandMiseAJourReussie()
    {
        var entity = CreateEnseignant();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateEnseignant();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Enseignant>>(),
            "kouroukan.events",
            "entity.updated.enseignant",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NePubliePas_SiEchec()
    {
        var entity = CreateEnseignant();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.UpdateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Enseignant>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_RetourneFalse_QuandInexistant()
    {
        var entity = CreateEnseignant();

        _repoMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.UpdateAsync(entity);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_Lance_InvalidOperationException_SiModeRemunerationInvalide()
    {
        var entity = CreateEnseignant(modeRemuneration: "INVALIDE");

        var act = async () => await _sut.UpdateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*mode de remuneration*");
    }

    // ─── DeleteAsync ───

    [Fact]
    public async Task DeleteAsync_RetourneTrue_QuandSuppressionReussie()
    {
        _repoMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.DeleteAsync(1);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_PublieEvenement_SiReussite()
    {
        _repoMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.DeleteAsync(1);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityDeletedEvent<Enseignant>>(),
            "kouroukan.events",
            "entity.deleted.enseignant",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_RetourneFalse_QuandInexistant()
    {
        _repoMock.Setup(r => r.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.DeleteAsync(999);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_NePubliePas_SiEchec()
    {
        _repoMock.Setup(r => r.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _sut.DeleteAsync(999);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityDeletedEvent<Enseignant>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── Helper ───

    private static Enseignant CreateEnseignant(
        int id = 1,
        string matricule = "MAT-001",
        string modeRemuneration = "Forfait")
    {
        return new Enseignant
        {
            Id = id,
            Name = "Mamadou Diallo",
            Description = "Professeur de mathematiques",
            Matricule = matricule,
            Specialite = "Mathematiques",
            DateEmbauche = new DateTime(2020, 9, 1),
            ModeRemuneration = modeRemuneration,
            MontantForfait = 500000m,
            Telephone = "+224 620 00 00 00",
            Email = "mamadou.diallo@test.com",
            StatutEnseignant = "Actif",
            SoldeCongesAnnuel = 30,
            TypeId = 1,
            UserId = 1
        };
    }
}
