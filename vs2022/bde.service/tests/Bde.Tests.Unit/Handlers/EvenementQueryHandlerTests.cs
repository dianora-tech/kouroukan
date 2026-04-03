using FluentAssertions;
using GnDapper.Models;
using Bde.Application.Handlers;
using Bde.Application.Queries;
using Bde.Domain.Entities;
using Bde.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Bde.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour EvenementQueryHandler.
/// </summary>
public sealed class EvenementQueryHandlerTests
{
    private readonly Mock<IEvenementService> _serviceMock;
    private readonly EvenementQueryHandler _sut;

    public EvenementQueryHandlerTests()
    {
        _serviceMock = new Mock<IEvenementService>();
        _sut = new EvenementQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneEvenement()
    {
        var evenement = new Evenement { Id = 1, Name = "Gala", StatutEvenement = "Planifie" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evenement);

        var result = await _sut.Handle(new GetEvenementByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistant()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Evenement?)null);

        var result = await _sut.Handle(new GetEvenementByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var evenements = new List<Evenement>
        {
            new() { Id = 1, Name = "Gala" },
            new() { Id = 2, Name = "Tournoi" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(evenements);

        var result = await _sut.Handle(new GetAllEvenementsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Evenement>(
            new List<Evenement> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedEvenementsQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
