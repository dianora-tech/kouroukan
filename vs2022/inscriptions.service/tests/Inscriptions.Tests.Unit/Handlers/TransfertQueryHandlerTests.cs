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
/// Tests unitaires pour TransfertQueryHandler.
/// </summary>
public sealed class TransfertQueryHandlerTests
{
    private readonly Mock<ITransfertService> _serviceMock;
    private readonly TransfertQueryHandler _sut;

    public TransfertQueryHandlerTests()
    {
        _serviceMock = new Mock<ITransfertService>();
        _sut = new TransfertQueryHandler(_serviceMock.Object);
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Transfert>(
            new List<Transfert> { new() { Id = 1, Statut = "EnAttente" } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedTransfertsQuery(1, 20, "test", "createdAt"),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetPaged_SansRecherche_AppelleAvecNull()
    {
        var paged = new PagedResult<Transfert>(new List<Transfert>(), 0, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedTransfertsQuery(1, 20, null, null),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
