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
/// Tests unitaires pour SalleQueryHandler.
/// </summary>
public sealed class SalleQueryHandlerTests
{
    private readonly Mock<ISalleService> _serviceMock;
    private readonly SalleQueryHandler _sut;

    public SalleQueryHandlerTests()
    {
        _serviceMock = new Mock<ISalleService>();
        _sut = new SalleQueryHandler(_serviceMock.Object);
    }

    // ─── GetById ───

    [Fact]
    public async Task Handle_GetByIdQuery_RetourneSalle_QuandExiste()
    {
        var expected = new Salle { Id = 1, Name = "Salle 101" };

        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(new GetSalleByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetByIdQuery_RetourneNull_QuandInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Salle?)null);

        var result = await _sut.Handle(new GetSalleByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    // ─── GetAll ───

    [Fact]
    public async Task Handle_GetAllQuery_RetourneListe()
    {
        var salles = new List<Salle>
        {
            new() { Id = 1, Name = "Salle 101" },
            new() { Id = 2, Name = "Salle 102" }
        };

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(salles);

        var result = await _sut.Handle(new GetAllSallesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPagedQuery_RetourneResultatPagine()
    {
        var paged = new PagedResult<Salle>(
            new List<Salle> { new() { Id = 1, Name = "Salle 101" } },
            1, 1, 20);

        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedSallesQuery(1, 20, null, null, null), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
