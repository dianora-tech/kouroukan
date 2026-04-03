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
/// Tests unitaires pour BadgeageQueryHandler.
/// </summary>
public sealed class BadgeageQueryHandlerTests
{
    private readonly Mock<IBadgeageService> _serviceMock;
    private readonly BadgeageQueryHandler _sut;

    public BadgeageQueryHandlerTests()
    {
        _serviceMock = new Mock<IBadgeageService>();
        _sut = new BadgeageQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneBadgeage()
    {
        var badgeage = new Badgeage { Id = 1, EleveId = 10, PointAcces = "Entree" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(badgeage);

        var result = await _sut.Handle(new GetBadgeageByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistant()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Badgeage?)null);

        var result = await _sut.Handle(new GetBadgeageByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var badgeages = new List<Badgeage>
        {
            new() { Id = 1, EleveId = 10 },
            new() { Id = 2, EleveId = 20 },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(badgeages);

        var result = await _sut.Handle(new GetAllBadgeagesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Badgeage>(
            new List<Badgeage> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedBadgeagesQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
