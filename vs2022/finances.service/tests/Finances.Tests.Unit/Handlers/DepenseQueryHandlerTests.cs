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
/// Tests unitaires pour DepenseQueryHandler.
/// </summary>
public sealed class DepenseQueryHandlerTests
{
    private readonly Mock<IDepenseService> _serviceMock;
    private readonly DepenseQueryHandler _sut;

    public DepenseQueryHandlerTests()
    {
        _serviceMock = new Mock<IDepenseService>();
        _sut = new DepenseQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneDepense()
    {
        var entity = new Depense { Id = 1, Categorie = "Fournitures" };
        _serviceMock.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var result = await _sut.Handle(new GetDepenseByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistante()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Depense?)null);

        var result = await _sut.Handle(new GetDepenseByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var list = new List<Depense> { new() { Id = 1 }, new() { Id = 2 } };
        _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);

        var result = await _sut.Handle(new GetAllDepensesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Depense>(new List<Depense> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock.Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>())).ReturnsAsync(paged);

        var result = await _sut.Handle(new GetPagedDepensesQuery(1, 20, "test", null, "createdAt"), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
