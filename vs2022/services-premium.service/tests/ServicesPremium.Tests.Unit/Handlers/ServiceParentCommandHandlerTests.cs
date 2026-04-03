using FluentAssertions;
using Moq;
using ServicesPremium.Application.Commands;
using ServicesPremium.Application.Handlers;
using ServicesPremium.Domain.Entities;
using ServicesPremium.Domain.Ports.Input;
using Xunit;

namespace ServicesPremium.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour ServiceParentCommandHandler.
/// </summary>
public sealed class ServiceParentCommandHandlerTests
{
    private readonly Mock<IServiceParentService> _serviceMock;
    private readonly ServiceParentCommandHandler _sut;

    public ServiceParentCommandHandlerTests()
    {
        _serviceMock = new Mock<IServiceParentService>();
        _sut = new ServiceParentCommandHandler(_serviceMock.Object);
    }

    // --- Create ---

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneServiceParent()
    {
        var command = new CreateServiceParentCommand(
            TypeId: 1,
            Name: "Service SMS",
            Description: "Alertes SMS",
            Code: "SVC-SMS-001",
            Tarif: 50000m,
            Periodicite: "Mensuel",
            EstActif: true,
            PeriodeEssaiJours: 30,
            TarifDegressif: false,
            UserId: 1);

        var expected = new ServiceParent
        {
            Id = 42,
            TypeId = 1,
            Name = "Service SMS",
            Code = "SVC-SMS-001",
            Periodicite = "Mensuel"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<ServiceParent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<ServiceParent>(e =>
                e.TypeId == 1 &&
                e.Name == "Service SMS" &&
                e.Code == "SVC-SMS-001" &&
                e.Tarif == 50000m &&
                e.Periodicite == "Mensuel" &&
                e.PeriodeEssaiJours == 30),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // --- Update ---

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateServiceParentCommand(
            Id: 1,
            TypeId: 1,
            Name: "Service SMS v2",
            Description: null,
            Code: "SVC-SMS-001",
            Tarif: 75000m,
            Periodicite: "Annuel",
            EstActif: true,
            PeriodeEssaiJours: null,
            TarifDegressif: true,
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<ServiceParent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<ServiceParent>(e => e.Id == 1 && e.Tarif == 75000m && e.TarifDegressif),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistant()
    {
        var command = new UpdateServiceParentCommand(
            Id: 999, TypeId: 1, Name: "N", Description: null, Code: "C",
            Tarif: 0, Periodicite: "Mensuel", EstActif: false,
            PeriodeEssaiJours: null, TarifDegressif: false, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<ServiceParent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // --- Delete ---

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteServiceParentCommand(1);

        _serviceMock
            .Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteCommand_RetourneFalse_SiInexistant()
    {
        var command = new DeleteServiceParentCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
