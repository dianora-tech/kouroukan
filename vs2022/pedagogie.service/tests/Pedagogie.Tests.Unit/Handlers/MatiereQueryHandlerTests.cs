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
/// Tests unitaires pour MatiereQueryHandler.
/// </summary>
public sealed class MatiereQueryHandlerTests
{
    private readonly Mock<IMatiereService> _serviceMock;
    private readonly MatiereQueryHandler _sut;

    public MatiereQueryHandlerTests()
    {
        _serviceMock = new Mock<IMatiereService>();
        _sut = new MatiereQueryHandler(_serviceMock.Object);
    }

    // ─── GetById ───

    [Fact]
    public async Task Handle_GetByIdQuery_RetourneMatiere_QuandExiste()
    {
        var expected = new Matiere { Id = 1, Name = "Mathematiques", Code = "MATH" };

        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(new GetMatiereByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetByIdQuery_RetourneNull_QuandInexistante()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Matiere?)null);

        var result = await _sut.Handle(new GetMatiereByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    // ─── GetAll ───

    [Fact]
    public async Task Handle_GetAllQuery_RetourneListe()
    {
        var matieres = new List<Matiere>
        {
            new() { Id = 1, Name = "Mathematiques" },
            new() { Id = 2, Name = "Francais" }
        };

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(matieres);

        var result = await _sut.Handle(new GetAllMatieresQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPagedQuery_RetourneResultatPagine()
    {
        var paged = new PagedResult<Matiere>(
            new List<Matiere> { new() { Id = 1, Name = "Mathematiques" } },
            1, 1, 20);

        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedMatieresQuery(1, 20, null, null, null), CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
