using FluentAssertions;
using GnDapper.Models;
using Moq;
using ServicesPremium.Application.Handlers;
using ServicesPremium.Application.Queries;
using ServicesPremium.Domain.Entities;
using ServicesPremium.Domain.Ports.Input;
using Xunit;

namespace ServicesPremium.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour SouscriptionQueryHandler.
/// </summary>
public sealed class SouscriptionQueryHandlerTests
{
    private readonly Mock<ISouscriptionService> _serviceMock;
    private readonly SouscriptionQueryHandler _sut;

    public SouscriptionQueryHandlerTests()
    {
        _serviceMock = new Mock<ISouscriptionService>();
        _sut = new SouscriptionQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneSouscription()
    {
        var entity = new Souscription { Id = 1, StatutSouscription = "Active" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.Handle(new GetSouscriptionByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Souscription?)null);

        var result = await _sut.Handle(new GetSouscriptionByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var list = new List<Souscription>
        {
            new() { Id = 1, StatutSouscription = "Active" },
            new() { Id = 2, StatutSouscription = "Expiree" }
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var result = await _sut.Handle(new GetAllSouscriptionsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Souscription>(
            new List<Souscription> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedSouscriptionsQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
