using Finances.Application.Commands;
using Finances.Application.Handlers;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using FluentAssertions;
using Moq;
using Xunit;

namespace Finances.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour FactureCommandHandler.
/// </summary>
public sealed class FactureCommandHandlerTests
{
    private readonly Mock<IFactureService> _serviceMock;
    private readonly FactureCommandHandler _sut;

    public FactureCommandHandlerTests()
    {
        _serviceMock = new Mock<IFactureService>();
        _sut = new FactureCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneFacture()
    {
        var command = new CreateFactureCommand(
            TypeId: 1, EleveId: 10, ParentId: 5, AnneeScolaireId: 1,
            MontantTotal: 150000m, MontantPaye: 0m,
            DateEmission: new DateTime(2025, 9, 1), DateEcheance: new DateTime(2025, 12, 31),
            StatutFacture: "Emise", NumeroFacture: "FAC-001", UserId: 1);

        var expected = new Facture { Id = 42, NumeroFacture = "FAC-001" };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Facture>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Facture>(e => e.EleveId == 10 && e.NumeroFacture == "FAC-001"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateFactureCommand(
            Id: 1, TypeId: 1, EleveId: 10, ParentId: 5, AnneeScolaireId: 1,
            MontantTotal: 200000m, MontantPaye: 50000m,
            DateEmission: new DateTime(2025, 9, 1), DateEcheance: new DateTime(2025, 12, 31),
            StatutFacture: "PartPaye", NumeroFacture: "FAC-001", UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Facture>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _sut.Handle(new DeleteFactureCommand(1), CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
