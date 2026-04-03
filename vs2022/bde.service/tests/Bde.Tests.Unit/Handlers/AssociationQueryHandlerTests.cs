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
/// Tests unitaires pour AssociationQueryHandler.
/// </summary>
public sealed class AssociationQueryHandlerTests
{
    private readonly Mock<IAssociationService> _serviceMock;
    private readonly AssociationQueryHandler _sut;

    public AssociationQueryHandlerTests()
    {
        _serviceMock = new Mock<IAssociationService>();
        _sut = new AssociationQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneAssociation()
    {
        var association = new Association { Id = 1, Name = "BDE", Statut = "Active" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(association);

        var result = await _sut.Handle(new GetAssociationByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Association?)null);

        var result = await _sut.Handle(new GetAssociationByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var associations = new List<Association>
        {
            new() { Id = 1, Name = "BDE", Statut = "Active" },
            new() { Id = 2, Name = "Club Sport", Statut = "Active" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(associations);

        var result = await _sut.Handle(new GetAllAssociationsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Association>(
            new List<Association> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedAssociationsQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
