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
/// Tests unitaires pour AffectationEnseignantQueryHandler.
/// </summary>
public sealed class AffectationEnseignantQueryHandlerTests
{
    private readonly Mock<IAffectationEnseignantService> _serviceMock;
    private readonly AffectationEnseignantQueryHandler _sut;

    public AffectationEnseignantQueryHandlerTests()
    {
        _serviceMock = new Mock<IAffectationEnseignantService>();
        _sut = new AffectationEnseignantQueryHandler(_serviceMock.Object);
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPagedQuery_RetourneResultatPagine()
    {
        var paged = new PagedResult<AffectationEnseignant>(
            new List<AffectationEnseignant>
            {
                new() { Id = 1, LiaisonId = 10, ClasseId = 5, MatiereId = 3 }
            }, 1, 1, 20);

        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedAffectationsEnseignantQuery(1, 20, null, null, null, null, null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetPagedQuery_AvecFiltres_AppelleServiceAvecFiltres()
    {
        var paged = new PagedResult<AffectationEnseignant>(
            new List<AffectationEnseignant>(), 0, 1, 20);

        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, 10, 5, 3, 1, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedAffectationsEnseignantQuery(1, 20, 10, 5, 3, 1, null),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
        _serviceMock.Verify(s => s.GetPagedAsync(
            1, 20, 10, 5, 3, 1, null, It.IsAny<CancellationToken>()), Times.Once);
    }
}
