using Finances.Application.Commands;
using Finances.Application.Handlers;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using FluentAssertions;
using Moq;
using Xunit;

namespace Finances.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour RemunerationEnseignantCommandHandler.
/// </summary>
public sealed class RemunerationEnseignantCommandHandlerTests
{
    private readonly Mock<IRemunerationEnseignantService> _serviceMock;
    private readonly RemunerationEnseignantCommandHandler _sut;

    public RemunerationEnseignantCommandHandlerTests()
    {
        _serviceMock = new Mock<IRemunerationEnseignantService>();
        _sut = new RemunerationEnseignantCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneRemuneration()
    {
        var command = new CreateRemunerationEnseignantCommand(
            EnseignantId: 10, Mois: 9, Annee: 2025, ModeRemuneration: "Forfait",
            MontantForfait: 2000000m, NombreHeures: null, TauxHoraire: null,
            StatutPaiement: "EnAttente", DateValidation: null, ValidateurId: null, UserId: 1);

        var expected = new RemunerationEnseignant { Id = 42, EnseignantId = 10 };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<RemunerationEnseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<RemunerationEnseignant>(e => e.EnseignantId == 10 && e.ModeRemuneration == "Forfait"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateRemunerationEnseignantCommand(
            Id: 1, EnseignantId: 10, Mois: 9, Annee: 2025, ModeRemuneration: "Heures",
            MontantForfait: null, NombreHeures: 40m, TauxHoraire: 50000m,
            StatutPaiement: "Valide", DateValidation: DateTime.Today, ValidateurId: 2, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<RemunerationEnseignant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _sut.Handle(new DeleteRemunerationEnseignantCommand(1), CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
