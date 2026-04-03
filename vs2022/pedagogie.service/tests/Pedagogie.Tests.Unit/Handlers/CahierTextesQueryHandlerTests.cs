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
/// Tests unitaires pour CahierTextesQueryHandler.
/// </summary>
public sealed class CahierTextesQueryHandlerTests
{
    private readonly Mock<ICahierTextesService> _serviceMock;
    private readonly CahierTextesQueryHandler _sut;

    public CahierTextesQueryHandlerTests()
    {
        _serviceMock = new Mock<ICahierTextesService>();
        _sut = new CahierTextesQueryHandler(_serviceMock.Object);
    }

    // ─── GetById ───

    [Fact]
    public async Task Handle_GetByIdQuery_RetourneCahierTextes_QuandExiste()
    {
        var expected = new CahierTextes { Id = 1, Name = "Cours du 01/09" };

        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(new GetCahierTextesByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetByIdQuery_RetourneNull_QuandInexistant()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CahierTextes?)null);

        var result = await _sut.Handle(new GetCahierTextesByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    // ─── GetAll ───

    [Fact]
    public async Task Handle_GetAllQuery_RetourneListe()
    {
        var cahiers = new List<CahierTextes>
        {
            new() { Id = 1, Name = "Cours du 01/09" },
            new() { Id = 2, Name = "Cours du 08/09" }
        };

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cahiers);

        var result = await _sut.Handle(new GetAllCahiersTextesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPagedQuery_RetourneResultatPagine()
    {
        var paged = new PagedResult<CahierTextes>(
            new List<CahierTextes> { new() { Id = 1, Name = "Cours du 01/09" } },
            1, 1, 20);

        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedCahiersTextesQuery(1, 20, null, null), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
