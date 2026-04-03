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
/// Tests unitaires pour FactureQueryHandler.
/// </summary>
public sealed class FactureQueryHandlerTests
{
    private readonly Mock<IFactureService> _serviceMock;
    private readonly FactureQueryHandler _sut;

    public FactureQueryHandlerTests()
    {
        _serviceMock = new Mock<IFactureService>();
        _sut = new FactureQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneFacture()
    {
        var entity = new Facture { Id = 1, NumeroFacture = "FAC-001" };
        _serviceMock.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var result = await _sut.Handle(new GetFactureByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var list = new List<Facture> { new() { Id = 1 }, new() { Id = 2 } };
        _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);

        var result = await _sut.Handle(new GetAllFacturesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<Facture>(new List<Facture> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock.Setup(s => s.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>())).ReturnsAsync(paged);

        var result = await _sut.Handle(new GetPagedFacturesQuery(1, 20, null, null, null), CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }
}
