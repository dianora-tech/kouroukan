using FluentAssertions;
using GnDapper.Models;
using Presences.Application.Handlers;
using Presences.Application.Queries;
using Presences.Domain.Entities;
using Presences.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Presences.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour AppelQueryHandler.
/// </summary>
public sealed class AppelQueryHandlerTests
{
    private readonly Mock<IAppelService> _serviceMock;
    private readonly AppelQueryHandler _sut;

    public AppelQueryHandlerTests()
    {
        _serviceMock = new Mock<IAppelService>();
        _sut = new AppelQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneAppel()
    {
        var appel = new Appel { Id = 1, ClasseId = 5, EnseignantId = 3 };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appel);

        var result = await _sut.Handle(new GetAppelByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistant()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appel?)null);

        var result = await _sut.Handle(new GetAppelByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var appels = new List<Appel>
        {
            new() { Id = 1, ClasseId = 5 },
            new() { Id = 2, ClasseId = 6 },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(appels);

        var result = await _sut.Handle(new GetAllAppelsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Appel>(
            new List<Appel> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedAppelsQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
