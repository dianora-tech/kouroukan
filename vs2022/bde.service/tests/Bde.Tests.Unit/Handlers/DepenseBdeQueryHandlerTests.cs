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
/// Tests unitaires pour DepenseBdeQueryHandler.
/// </summary>
public sealed class DepenseBdeQueryHandlerTests
{
    private readonly Mock<IDepenseBdeService> _serviceMock;
    private readonly DepenseBdeQueryHandler _sut;

    public DepenseBdeQueryHandlerTests()
    {
        _serviceMock = new Mock<IDepenseBdeService>();
        _sut = new DepenseBdeQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneDepense()
    {
        var depense = new DepenseBde { Id = 1, Name = "Achat", Montant = 100000m };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(depense);

        var result = await _sut.Handle(new GetDepenseBdeByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DepenseBde?)null);

        var result = await _sut.Handle(new GetDepenseBdeByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var depenses = new List<DepenseBde>
        {
            new() { Id = 1, Name = "Achat materiel" },
            new() { Id = 2, Name = "Location salle" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(depenses);

        var result = await _sut.Handle(new GetAllDepensesBdeQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<DepenseBde>(
            new List<DepenseBde> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedDepensesBdeQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
