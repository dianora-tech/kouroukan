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
/// Tests unitaires pour DossierAdmissionQueryHandler.
/// </summary>
public sealed class DossierAdmissionQueryHandlerTests
{
    private readonly Mock<IDossierAdmissionService> _serviceMock;
    private readonly DossierAdmissionQueryHandler _sut;

    public DossierAdmissionQueryHandlerTests()
    {
        _serviceMock = new Mock<IDossierAdmissionService>();
        _sut = new DossierAdmissionQueryHandler(_serviceMock.Object);
    }

    // ─── GetById ───

    [Fact]
    public async Task Handle_GetById_RetourneDossier()
    {
        var dossier = new DossierAdmission { Id = 1, StatutDossier = "EnEtude" };
        _serviceMock
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dossier);

        var result = await _sut.Handle(new GetDossierAdmissionByIdQuery(1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.StatutDossier.Should().Be("EnEtude");
    }

    [Fact]
    public async Task Handle_GetById_RetourneNull_SiInexistant()
    {
        _serviceMock
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DossierAdmission?)null);

        var result = await _sut.Handle(new GetDossierAdmissionByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    // ─── GetAll ───

    [Fact]
    public async Task Handle_GetAll_RetourneListe()
    {
        var dossiers = new List<DossierAdmission>
        {
            new() { Id = 1, StatutDossier = "EnEtude" },
            new() { Id = 2, StatutDossier = "Admis" },
        };
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(dossiers);

        var result = await _sut.Handle(new GetAllDossierAdmissionsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_GetAll_RetourneListeVide()
    {
        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DossierAdmission>());

        var result = await _sut.Handle(new GetAllDossierAdmissionsQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    // ─── GetPaged ───

    [Fact]
    public async Task Handle_GetPaged_RetourneResultatPagine()
    {
        var paged = new PagedResult<DossierAdmission>(
            new List<DossierAdmission> { new() { Id = 1 } }, 1, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, "test", null, "createdAt", It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedDossierAdmissionsQuery(1, 20, "test", "createdAt", null),
            CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_GetPaged_AvecTypeId_PasseLeFiltre()
    {
        var paged = new PagedResult<DossierAdmission>(new List<DossierAdmission>(), 0, 1, 20);
        _serviceMock
            .Setup(s => s.GetPagedAsync(1, 20, null, 5, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.Handle(
            new GetPagedDossierAdmissionsQuery(1, 20, null, null, 5),
            CancellationToken.None);

        result.Items.Should().BeEmpty();
        _serviceMock.Verify(s => s.GetPagedAsync(1, 20, null, 5, null, It.IsAny<CancellationToken>()), Times.Once);
    }
}
