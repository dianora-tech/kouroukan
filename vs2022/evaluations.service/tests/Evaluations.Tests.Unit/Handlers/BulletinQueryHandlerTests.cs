using FluentAssertions;
using GnDapper.Models;
using Evaluations.Application.Handlers;
using Evaluations.Application.Queries;
using Evaluations.Domain.Entities;
using Evaluations.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Evaluations.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour BulletinQueryHandler.
/// </summary>
public sealed class BulletinQueryHandlerTests
{
    private readonly Mock<IBulletinService> _serviceMock;
    private readonly BulletinQueryHandler _sut;

    public BulletinQueryHandlerTests()
    {
        _serviceMock = new Mock<IBulletinService>();
        _sut = new BulletinQueryHandler(_serviceMock.Object);
    }

    // ─── GetById ───

    [Fact]
    public async Task Handle_GetById_RetourneBulletin()
    {
        var bulletin = new Bulletin { Id = 1, EleveId = 5, MoyenneGenerale = 14.5m };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bulletin);

        var result = await _sut.Handle(new GetBulletinByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistant()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Bulletin?)null);

        var result = await _sut.Handle(new GetBulletinByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    // ─── GetAll ───

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var bulletins = new List<Bulletin>
        {
            new() { Id = 1, MoyenneGenerale = 14m },
            new() { Id = 2, MoyenneGenerale = 12m },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(bulletins);

        var result = await _sut.Handle(new GetAllBulletinsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Bulletin>(
            new List<Bulletin> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedBulletinsQuery(1, 20, "test", "createdAt"),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
