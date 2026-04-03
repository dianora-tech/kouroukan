using FluentAssertions;
using Inscriptions.Application.Commands;
using Inscriptions.Application.Handlers;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Inscriptions.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour LiaisonParentCommandHandler.
/// </summary>
public sealed class LiaisonParentCommandHandlerTests
{
    private readonly Mock<ILiaisonParentService> _serviceMock;
    private readonly LiaisonParentCommandHandler _sut;

    public LiaisonParentCommandHandlerTests()
    {
        _serviceMock = new Mock<ILiaisonParentService>();
        _sut = new LiaisonParentCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneLiaison()
    {
        var command = new CreateLiaisonParentCommand(
            ParentUserId: 5,
            EleveId: 10,
            CompanyId: 1);

        var expected = new LiaisonParent
        {
            Id = 42,
            ParentUserId = 5,
            EleveId = 10,
            CompanyId = 1,
            Statut = "Active"
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<LiaisonParent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<LiaisonParent>(l =>
                l.ParentUserId == 5 &&
                l.EleveId == 10 &&
                l.CompanyId == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteLiaisonParentCommand(1);

        _serviceMock
            .Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteCommand_RetourneFalse_SiInexistante()
    {
        var command = new DeleteLiaisonParentCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
