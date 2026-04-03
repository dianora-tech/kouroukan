using Finances.Application.Commands;
using Finances.Application.Handlers;
using Finances.Domain.Entities;
using Finances.Domain.Ports.Input;
using FluentAssertions;
using Moq;
using Xunit;

namespace Finances.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour MoyenPaiementCommandHandler.
/// </summary>
public sealed class MoyenPaiementCommandHandlerTests
{
    private readonly Mock<IMoyenPaiementService> _serviceMock;
    private readonly MoyenPaiementCommandHandler _sut;

    public MoyenPaiementCommandHandlerTests()
    {
        _serviceMock = new Mock<IMoyenPaiementService>();
        _sut = new MoyenPaiementCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneMoyenPaiement()
    {
        var command = new CreateMoyenPaiementCommand(
            CompanyId: 1, Operateur: "OrangeMoney", NumeroCompte: "622000000",
            CodeMarchand: "MARC-001", Libelle: "Orange Money Ecole");

        var expected = new MoyenPaiement { Id = 42, Operateur = "OrangeMoney" };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<MoyenPaiement>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
    }

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateMoyenPaiementCommand(
            Id: 1, CompanyId: 1, Operateur: "MTNMoMo", NumeroCompte: "622111111",
            CodeMarchand: "MARC-002", Libelle: "MTN MoMo Ecole", EstActif: true);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<MoyenPaiement>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _sut.Handle(new DeleteMoyenPaiementCommand(1), CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
