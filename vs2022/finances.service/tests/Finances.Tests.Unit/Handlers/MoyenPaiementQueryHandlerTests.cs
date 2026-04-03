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
/// Tests unitaires pour MoyenPaiementQueryHandler.
/// </summary>
public sealed class MoyenPaiementQueryHandlerTests
{
    private readonly Mock<IMoyenPaiementService> _serviceMock;
    private readonly MoyenPaiementQueryHandler _sut;

    public MoyenPaiementQueryHandlerTests()
    {
        _serviceMock = new Mock<IMoyenPaiementService>();
        _sut = new MoyenPaiementQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<MoyenPaiement>(
            new List<MoyenPaiement> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedMoyensPaiementQuery(1, 20, null, null), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
