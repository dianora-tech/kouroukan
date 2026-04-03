using FluentAssertions;
using GnDapper.Models;
using Documents.Application.Handlers;
using Documents.Application.Queries;
using Documents.Domain.Entities;
using Documents.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Documents.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour ModeleDocumentQueryHandler.
/// </summary>
public sealed class ModeleDocumentQueryHandlerTests
{
    private readonly Mock<IModeleDocumentService> _serviceMock;
    private readonly ModeleDocumentQueryHandler _sut;

    public ModeleDocumentQueryHandlerTests()
    {
        _serviceMock = new Mock<IModeleDocumentService>();
        _sut = new ModeleDocumentQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneModeleDocument()
    {
        var entity = new ModeleDocument { Id = 1, Code = "BULL_NOTES" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.Handle(new GetModeleDocumentByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistant()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ModeleDocument?)null);

        var result = await _sut.Handle(new GetModeleDocumentByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var entities = new List<ModeleDocument>
        {
            new() { Id = 1, Code = "BULL_NOTES" },
            new() { Id = 2, Code = "ATT_SCOL" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _sut.Handle(new GetAllModeleDocumentsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<ModeleDocument>(
            new List<ModeleDocument> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedModeleDocumentsQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
