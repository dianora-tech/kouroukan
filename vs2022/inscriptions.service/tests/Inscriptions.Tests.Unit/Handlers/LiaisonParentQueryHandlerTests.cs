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
/// Tests unitaires pour LiaisonParentQueryHandler.
/// </summary>
public sealed class LiaisonParentQueryHandlerTests
{
    private readonly Mock<ILiaisonParentService> _serviceMock;
    private readonly LiaisonParentQueryHandler _sut;

    public LiaisonParentQueryHandlerTests()
    {
        _serviceMock = new Mock<ILiaisonParentService>();
        _sut = new LiaisonParentQueryHandler(_serviceMock.Object);
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<LiaisonParent>(
            new List<LiaisonParent> { new() { Id = 1, ParentUserId = 5 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, 5, null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedLiaisonsParentQuery(1, 20, 5, null, "createdAt"),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetPaged_AvecCompanyId_PasseLeFiltre()
    {
        var paged = new PagedResult<LiaisonParent>(new List<LiaisonParent>(), 0, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, 3, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedLiaisonsParentQuery(1, 20, null, 3, null),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
        _serviceMock.Verify(s => s.GetPagedAsync(1, 20, null, 3, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_GetPaged_SansFiltres_AppelleAvecNulls()
    {
        var paged = new PagedResult<LiaisonParent>(new List<LiaisonParent>(), 0, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedLiaisonsParentQuery(1, 20, null, null, null),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
    }
}
