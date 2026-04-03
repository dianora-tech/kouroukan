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
/// Tests unitaires pour RemunerationEnseignantQueryHandler.
/// </summary>
public sealed class RemunerationEnseignantQueryHandlerTests
{
    private readonly Mock<IRemunerationEnseignantService> _serviceMock;
    private readonly RemunerationEnseignantQueryHandler _sut;

    public RemunerationEnseignantQueryHandlerTests()
    {
        _serviceMock = new Mock<IRemunerationEnseignantService>();
        _sut = new RemunerationEnseignantQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_GetById_RetourneRemuneration()
    {
        var entity = new RemunerationEnseignant { Id = 1, ModeRemuneration = "Forfait" };
        _serviceMock.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var result = await _sut.Handle(new GetRemunerationEnseignantByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var list = new List<RemunerationEnseignant> { new() { Id = 1 }, new() { Id = 2 } };
        _serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);

        var result = await _sut.Handle(new GetAllRemunerationsEnseignantsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<RemunerationEnseignant>(
            new List<RemunerationEnseignant> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock.Setup(s => s.GetPagedAsync(1, 20, null, null, It.IsAny<CancellationToken>())).ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedRemunerationsEnseignantsQuery(1, 20, null, null), CancellationToken.None);

        result.Items.Should().HaveCount(1);
    }
}
