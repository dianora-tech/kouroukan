using FluentAssertions;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Handlers;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Inscriptions.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour RadiationCommandHandler.
/// </summary>
public sealed class RadiationCommandHandlerTests
{
    private readonly Mock<IRadiationService> _serviceMock;
    private readonly RadiationCommandHandler _sut;

    public RadiationCommandHandlerTests()
    {
        _serviceMock = new Mock<IRadiationService>();
        _sut = new RadiationCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneRadiation()
    {
        var command = new CreateRadiationCommand(
            EleveId: 10,
            CompanyId: 1,
            Motif: "Absenteisme repete",
            Documents: null);

        var expected = new Radiation
        {
            Id = 42,
            EleveId = 10,
            CompanyId = 1,
            Motif = "Absenteisme repete"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Radiation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Radiation>(r =>
                r.EleveId == 10 &&
                r.CompanyId == 1 &&
                r.Motif == "Absenteisme repete"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateCommand_MappeDocuments()
    {
        var command = new CreateRadiationCommand(
            EleveId: 10,
            CompanyId: 1,
            Motif: "Exclusion",
            Documents: "{\"rapport\":\"rapport.pdf\"}");

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Radiation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Radiation { Id = 1 });

        await _sut.Handle(command, CancellationToken.None);

        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Radiation>(r => r.Documents == "{\"rapport\":\"rapport.pdf\"}"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
