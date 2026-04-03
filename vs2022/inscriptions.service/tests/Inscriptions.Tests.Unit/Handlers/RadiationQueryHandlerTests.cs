using FluentAssertions;
using GnDapper.Models;
using Inscriptions.Application.Handlers;
using Inscriptions.Application.Queries;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Inscriptions.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour RadiationQueryHandler.
/// </summary>
public sealed class RadiationQueryHandlerTests
{
    private readonly Mock<IRadiationService> _serviceMock;
    private readonly RadiationQueryHandler _sut;

    public RadiationQueryHandlerTests()
    {
        _serviceMock = new Mock<IRadiationService>();
        _sut = new RadiationQueryHandler(_serviceMock.Object);
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Radiation>(
            new List<Radiation> { new() { Id = 1, Motif = "Absenteisme" } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedRadiationsQuery(1, 20, "test", "createdAt"),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetPaged_SansRecherche_AppelleAvecNull()
    {
        var paged = new PagedResult<Radiation>(new List<Radiation>(), 0, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedRadiationsQuery(1, 20, null, null),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
