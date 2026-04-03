using FluentAssertions;
using GnDapper.Models;
using Inscriptions.Application.Handlers;
using Inscriptions.Application.Queries;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Inscriptions.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour AnneeScolaireQueryHandler.
/// </summary>
public sealed class AnneeScolaireQueryHandlerTests
{
    private readonly Mock<IAnneeScolaireService> _serviceMock;
    private readonly AnneeScolaireQueryHandler _sut;

    public AnneeScolaireQueryHandlerTests()
    {
        _serviceMock = new Mock<IAnneeScolaireService>();
        _sut = new AnneeScolaireQueryHandler(_serviceMock.Object);
    }

    // ─── GetById ───

    [Fact]
    public async Task Handle_GetById_RetourneAnneeScolaire()
    {
        var annee = new AnneeScolaire { Id = 1, Libelle = "2025-2026", EstActive = true };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(annee);

        var result = await _sut.Handle(new GetAnneeScolaireByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Libelle.Should().Be("2025-2026");
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AnneeScolaire?)null);

        var result = await _sut.Handle(new GetAnneeScolaireByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    // ─── GetAll ───

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var annees = new List<AnneeScolaire>
        {
            new() { Id = 1, Libelle = "2024-2025" },
            new() { Id = 2, Libelle = "2025-2026" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(annees);

        var result = await _sut.Handle(new GetAllAnneeScolairesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListeVide()
    {
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AnneeScolaire>());

        var result = await _sut.Handle(new GetAllAnneeScolairesQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<AnneeScolaire>(
            new List<AnneeScolaire> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "2025", "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedAnneeScolairesQuery(1, 20, "2025", "createdAt"),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetPaged_SansRecherche_AppelleAvecNull()
    {
        var paged = new PagedResult<AnneeScolaire>(new List<AnneeScolaire>(), 0, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedAnneeScolairesQuery(1, 20, null, null),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
