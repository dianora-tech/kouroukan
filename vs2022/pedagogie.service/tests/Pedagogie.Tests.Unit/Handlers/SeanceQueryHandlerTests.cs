using FluentAssertions;
using GnDapper.Models;
using Pedagogie.Application.Handlers;
using Pedagogie.Application.Queries;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Pedagogie.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour SeanceQueryHandler.
/// </summary>
public sealed class SeanceQueryHandlerTests
{
    private readonly Mock<ISeanceService> _serviceMock;
    private readonly SeanceQueryHandler _sut;

    public SeanceQueryHandlerTests()
    {
        _serviceMock = new Mock<ISeanceService>();
        _sut = new SeanceQueryHandler(_serviceMock.Object);
    }

    // ─── GetById ───

    [Fact]
    public async Task Handle_GetByIdQuery_RetourneSeance_QuandExiste()
    {
        var expected = new Seance { Id = 1, Name = "Maths 7A" };

        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(new GetSeanceByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetByIdQuery_RetourneNull_QuandInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Seance?)null);

        var result = await _sut.Handle(new GetSeanceByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    // ─── GetAll ───

    [Fact]
    public async Task Handle_GetAllQuery_RetourneListe()
    {
        var seances = new List<Seance>
        {
            new() { Id = 1, Name = "Maths 7A" },
            new() { Id = 2, Name = "Francais 7A" }
        };

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(seances);

        var result = await _sut.Handle(new GetAllSeancesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPagedQuery_RetourneResultatPagine()
    {
        var paged = new PagedResult<Seance>(
            new List<Seance> { new() { Id = 1, Name = "Maths 7A" } },
            1, 1, 20);

        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedSeancesQuery(1, 20, null, null), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
