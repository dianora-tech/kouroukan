using FluentAssertions;
using GnDapper.Models;
using Personnel.Application.Handlers;
using Personnel.Application.Queries;
using Personnel.Domain.Entities;
using Personnel.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Personnel.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour DemandeCongeQueryHandler.
/// </summary>
public sealed class DemandeCongeQueryHandlerTests
{
    private readonly Mock<IDemandeCongeService> _serviceMock;
    private readonly DemandeCongeQueryHandler _sut;

    public DemandeCongeQueryHandlerTests()
    {
        _serviceMock = new Mock<IDemandeCongeService>();
        _sut = new DemandeCongeQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneDemandeConge()
    {
        var demande = new DemandeConge { Id = 1, Name = "Conge annuel", StatutDemande = "Soumise" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(demande);

        var result = await _sut.Handle(new GetDemandeCongeByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DemandeConge?)null);

        var result = await _sut.Handle(new GetDemandeCongeByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var demandes = new List<DemandeConge>
        {
            new() { Id = 1, Name = "Conge annuel", StatutDemande = "Soumise" },
            new() { Id = 2, Name = "Conge maladie", StatutDemande = "ApprouveeN1" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(demandes);

        var result = await _sut.Handle(new GetAllDemandesCongesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListeVide_SiAucuneDemande()
    {
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DemandeConge>());

        var result = await _sut.Handle(new GetAllDemandesCongesQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<DemandeConge>(
            new List<DemandeConge> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "conge", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedDemandesCongesQuery(1, 20, "conge", null, "createdAt"),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetPaged_TransmetTousLesParametres()
    {
        var paged = new PagedResult<DemandeConge>(
            new List<DemandeConge>(), 0, 2, 10);
        _serviceMock
            .Setup(s => s.GetPagedAsync(2, 10, "maladie", 3, "dateDebut", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedDemandesCongesQuery(2, 10, "maladie", 3, "dateDebut"),
            CancellationToken.None);

        result.TotalCount.Should().Be(0);
        _serviceMock.Verify(s => s.GetPagedAsync(2, 10, "maladie", 3, "dateDebut", It.IsAny<CancellationToken>()), Times.Once);
    }
}
