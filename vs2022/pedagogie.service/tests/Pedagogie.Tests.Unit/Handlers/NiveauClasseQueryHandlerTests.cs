using FluentAssertions;
using GnDapper.Models;
using Pedagogie.Application.Handlers;
using Pedagogie.Application.Queries;
using Pedagogie.Domain.Entities;
using Pedagogie.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Pedagogie.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour NiveauClasseQueryHandler.
/// </summary>
public sealed class NiveauClasseQueryHandlerTests
{
    private readonly Mock<INiveauClasseService> _serviceMock;
    private readonly NiveauClasseQueryHandler _sut;

    public NiveauClasseQueryHandlerTests()
    {
        _serviceMock = new Mock<INiveauClasseService>();
        _sut = new NiveauClasseQueryHandler(_serviceMock.Object);
    }

    // ─── GetById ───

    [Fact]
    public async Task Handle_GetByIdQuery_RetourneNiveauClasse_QuandExiste()
    {
        var expected = new NiveauClasse { Id = 1, Name = "7eme annee", Code = "7E" };

        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(new GetNiveauClasseByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetByIdQuery_RetourneNull_QuandInexistant()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NiveauClasse?)null);

        var result = await _sut.Handle(new GetNiveauClasseByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    // ─── GetAll ───

    [Fact]
    public async Task Handle_GetAllQuery_RetourneListe()
    {
        var niveaux = new List<NiveauClasse>
        {
            new() { Id = 1, Name = "7eme annee" },
            new() { Id = 2, Name = "8eme annee" }
        };

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(niveaux);

        var result = await _sut.Handle(new GetAllNiveauClassesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPagedQuery_RetourneResultatPagine()
    {
        var paged = new PagedResult<NiveauClasse>(
            new List<NiveauClasse> { new() { Id = 1, Name = "7eme annee" } },
            1, 1, 20);

        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedNiveauClassesQuery(1, 20, null, null, null), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
