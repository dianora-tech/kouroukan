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
/// Tests unitaires pour CompetenceEnseignantQueryHandler.
/// </summary>
public sealed class CompetenceEnseignantQueryHandlerTests
{
    private readonly Mock<ICompetenceEnseignantService> _serviceMock;
    private readonly CompetenceEnseignantQueryHandler _sut;

    public CompetenceEnseignantQueryHandlerTests()
    {
        _serviceMock = new Mock<ICompetenceEnseignantService>();
        _sut = new CompetenceEnseignantQueryHandler(_serviceMock.Object);
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPagedQuery_RetourneResultatPagine()
    {
        var paged = new PagedResult<CompetenceEnseignant>(
            new List<CompetenceEnseignant>
            {
                new() { Id = 1, UserId = 10, MatiereId = 3, CycleEtude = "College" }
            }, 1, 1, 20);

        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedCompetencesEnseignantQuery(1, 20, null, null, null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetPagedQuery_AvecFiltres_AppelleServiceAvecFiltres()
    {
        var paged = new PagedResult<CompetenceEnseignant>(
            new List<CompetenceEnseignant>(), 0, 1, 20);

        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, 10, "College", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedCompetencesEnseignantQuery(1, 20, 10, "College", null),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
        _serviceMock.Verify(s => s.GetPagedAsync(
            1, 20, 10, "College", null, It.IsAny<CancellationToken>()), Times.Once);
    }
}
