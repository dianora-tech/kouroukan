using Finances.Application.Commands;
using Finances.Application.Handlers;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using FluentAssertions;
using Moq;
using Xunit;

namespace Finances.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour PaiementCommandHandler.
/// </summary>
public sealed class PaiementCommandHandlerTests
{
    private readonly Mock<IPaiementService> _serviceMock;
    private readonly PaiementCommandHandler _sut;

    public PaiementCommandHandlerTests()
    {
        _serviceMock = new Mock<IPaiementService>();
        _sut = new PaiementCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetournePaiement()
    {
        var command = new CreatePaiementCommand(
            TypeId: 1, FactureId: 10, MontantPaye: 50000m,
            DatePaiement: new DateTime(2025, 9, 15), MoyenPaiement: "Especes",
            ReferenceMobileMoney: null, StatutPaiement: "Confirme",
            CaissierId: 3, NumeroRecu: "REC-001", UserId: 1);

        var expected = new Paiement { Id = 42, NumeroRecu = "REC-001" };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Paiement>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
    }

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdatePaiementCommand(
            Id: 1, TypeId: 1, FactureId: 10, MontantPaye: 75000m,
            DatePaiement: DateTime.Today, MoyenPaiement: "OrangeMoney",
            ReferenceMobileMoney: "REF-123", StatutPaiement: "Confirme",
            CaissierId: null, NumeroRecu: "REC-001", UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Paiement>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _sut.Handle(new DeletePaiementCommand(1), CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
