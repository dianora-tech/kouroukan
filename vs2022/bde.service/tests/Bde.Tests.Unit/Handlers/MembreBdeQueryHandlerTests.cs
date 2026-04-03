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
/// Tests unitaires pour MembreBdeQueryHandler.
/// </summary>
public sealed class MembreBdeQueryHandlerTests
{
    private readonly Mock<IMembreBdeService> _serviceMock;
    private readonly MembreBdeQueryHandler _sut;

    public MembreBdeQueryHandlerTests()
    {
        _serviceMock = new Mock<IMembreBdeService>();
        _sut = new MembreBdeQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneMembre()
    {
        var membre = new MembreBde { Id = 1, Name = "Jean", RoleBde = "Membre" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(membre);

        var result = await _sut.Handle(new GetMembreBdeByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistant()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MembreBde?)null);

        var result = await _sut.Handle(new GetMembreBdeByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var membres = new List<MembreBde>
        {
            new() { Id = 1, Name = "Jean" },
            new() { Id = 2, Name = "Marie" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(membres);

        var result = await _sut.Handle(new GetAllMembresBdeQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<MembreBde>(
            new List<MembreBde> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedMembresBdeQuery(1, 20, "test", "createdAt"),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
