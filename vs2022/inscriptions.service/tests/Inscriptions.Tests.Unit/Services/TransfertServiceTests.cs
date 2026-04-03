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
/// Tests unitaires pour TransfertService.
/// </summary>
public sealed class TransfertServiceTests
{
    private readonly Mock<ITransfertRepository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<TransfertService>> _loggerMock;
    private readonly TransfertService _sut;

    public TransfertServiceTests()
    {
        _repoMock = new Mock<ITransfertRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<TransfertService>>();

        _sut = new TransfertService(
            _repoMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    // ─── GetByIdAsync ───

    [Fact]
    public async Task GetByIdAsync_RetourneTransfert_QuandExiste()
    {
        var transfert = CreateTransfert();
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transfert);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_RetourneNull_QuandInexistant()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transfert?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // ─── GetPagedAsync ───

    [Fact]
    public async Task GetPagedAsync_RetourneResultatPagine()
    {
        var paged = new PagedResult<Transfert>(
            new List<Transfert> { CreateTransfert() }, 1, 1, 20);
        _repoMock.Setup(r => r.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetPagedAsync(1, 20, null, null);

        result.Items.Should().HaveCount(1);
    }

    // ─── CreateAsync ───

    [Fact]
    public async Task CreateAsync_CreeLeTransfert_AvecStatutEnAttente()
    {
        var entity = CreateTransfert();

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Transfert>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.CreateAsync(entity);

        result.Should().NotBeNull();
        entity.Statut.Should().Be("EnAttente");
    }

    [Fact]
    public async Task CreateAsync_PublieEvenement_ApresCreation()
    {
        var entity = CreateTransfert();

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Transfert>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await _sut.CreateAsync(entity);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityCreatedEvent<Transfert>>(),
            "kouroukan.events",
            "entity.created.transfert",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Lance_InvalidOperationException_SiMemeEtablissement()
    {
        var entity = CreateTransfert();
        entity.CompanyCibleId = entity.CompanyOrigineId;

        var act = async () => await _sut.CreateAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*origine*cible*differents*");
    }

    // ─── AcceptAsync ───

    [Fact]
    public async Task AcceptAsync_RetourneTrue_QuandTransfertEnAttente()
    {
        var entity = CreateTransfert(statut: "EnAttente");
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Transfert>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.AcceptAsync(1);

        result.Should().BeTrue();
        entity.Statut.Should().Be("Accepte");
    }

    [Fact]
    public async Task AcceptAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateTransfert(statut: "EnAttente");
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Transfert>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.AcceptAsync(1);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Transfert>>(),
            "kouroukan.events",
            "entity.updated.transfert.accepted",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AcceptAsync_RetourneFalse_SiInexistant()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transfert?)null);

        var result = await _sut.AcceptAsync(999);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task AcceptAsync_Lance_InvalidOperationException_SiPasEnAttente()
    {
        var entity = CreateTransfert(statut: "Accepte");
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var act = async () => await _sut.AcceptAsync(1);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*en attente*accepte*");
    }

    // ─── RejectAsync ───

    [Fact]
    public async Task RejectAsync_RetourneTrue_QuandTransfertEnAttente()
    {
        var entity = CreateTransfert(statut: "EnAttente");
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Transfert>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.RejectAsync(1);

        result.Should().BeTrue();
        entity.Statut.Should().Be("Refuse");
    }

    [Fact]
    public async Task RejectAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateTransfert(statut: "EnAttente");
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Transfert>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.RejectAsync(1);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Transfert>>(),
            "kouroukan.events",
            "entity.updated.transfert.rejected",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RejectAsync_RetourneFalse_SiInexistant()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transfert?)null);

        var result = await _sut.RejectAsync(999);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task RejectAsync_Lance_InvalidOperationException_SiPasEnAttente()
    {
        var entity = CreateTransfert(statut: "Refuse");
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var act = async () => await _sut.RejectAsync(1);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*en attente*refuse*");
    }

    // ─── CompleteAsync ───

    [Fact]
    public async Task CompleteAsync_RetourneTrue_QuandTransfertAccepte()
    {
        var entity = CreateTransfert(statut: "Accepte");
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Transfert>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.CompleteAsync(1);

        result.Should().BeTrue();
        entity.Statut.Should().Be("Complete");
    }

    [Fact]
    public async Task CompleteAsync_PublieEvenement_SiReussite()
    {
        var entity = CreateTransfert(statut: "Accepte");
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Transfert>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _sut.CompleteAsync(1);

        _publisherMock.Verify(p => p.PublishAsync(
            It.IsAny<EntityUpdatedEvent<Transfert>>(),
            "kouroukan.events",
            "entity.updated.transfert.completed",
            It.IsAny<PublishOptions?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CompleteAsync_RetourneFalse_SiInexistant()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transfert?)null);

        var result = await _sut.CompleteAsync(999);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CompleteAsync_Lance_InvalidOperationException_SiPasAccepte()
    {
        var entity = CreateTransfert(statut: "EnAttente");
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var act = async () => await _sut.CompleteAsync(1);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*accepte*complete*");
    }

    // ─── Helper ───

    private static Transfert CreateTransfert(string statut = "EnAttente")
    {
        return new Transfert
        {
            Id = 1,
            EleveId = 10,
            CompanyOrigineId = 1,
            CompanyCibleId = 2,
            Statut = statut,
            Motif = "Demenagement",
            DateDemande = DateTime.UtcNow
        };
    }
}
