using Finances.Application.Handlers;
using Finances.Application.Queries;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using FluentAssertions;
using GnDapper.Models;
using Moq;
using Xunit;

namespace Finances.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour PalierFamilialQueryHandler.
/// </summary>
public sealed class PalierFamilialQueryHandlerTests
{
    private readonly Mock<IPalierFamilialService> _serviceMock;
    private readonly PalierFamilialQueryHandler _sut;

    public PalierFamilialQueryHandlerTests()
    {
        _serviceMock = new Mock<IPalierFamilialService>();
        _sut = new PalierFamilialQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<PalierFamilial>(
            new List<PalierFamilial> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedPaliersFamiliauxQuery(1, 20, null, null), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
