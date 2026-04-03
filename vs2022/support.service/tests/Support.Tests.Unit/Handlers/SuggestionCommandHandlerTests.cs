using FluentAssertions;
using Support.Application.Commands;
using Support.Application.Handlers;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;
using Moq;
using Xunit;

namespace Support.Tests.Unit.Handlers;

/// <summary>
/// Tests unitaires pour SuggestionCommandHandler.
/// </summary>
public sealed class SuggestionCommandHandlerTests
{
    private readonly Mock<ISuggestionService> _serviceMock;
    private readonly SuggestionCommandHandler _sut;

    public SuggestionCommandHandlerTests()
    {
        _serviceMock = new Mock<ISuggestionService>();
        _sut = new SuggestionCommandHandler(_serviceMock.Object);
    }

    // ─── Create ───

    [Fact]
    public async Task Handle_CreateCommand_AppelleService_EtRetourneSuggestion()
    {
        var command = new CreateSuggestionCommand(
            Name: "Suggestion test",
            Description: "Description",
            TypeId: 1,
            AuteurId: 10,
            Titre: "Ajouter mode sombre",
            Contenu: "Il serait bien d'avoir un mode sombre",
            ModuleConcerne: "UI",
            UserId: 1);

        var expected = new Suggestion
        {
            Id = 42,
            Name = "Suggestion test",
            Titre = "Ajouter mode sombre",
            Contenu = "Il serait bien d'avoir un mode sombre",
            AuteurId = 10
        };

        _serviceMock
            .Setup(s => s.CreateAsync(It.IsAny<Suggestion>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<Suggestion>(sg =>
                sg.Name == "Suggestion test" &&
                sg.Titre == "Ajouter mode sombre" &&
                sg.Contenu == "Il serait bien d'avoir un mode sombre" &&
                sg.AuteurId == 10 &&
                sg.ModuleConcerne == "UI" &&
                sg.UserId == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Update ───

    [Fact]
    public async Task Handle_UpdateCommand_AppelleService_EtRetourneTrue()
    {
        var command = new UpdateSuggestionCommand(
            Id: 1,
            Name: "Suggestion modifiee",
            Description: "Desc",
            TypeId: 1,
            AuteurId: 10,
            Titre: "Titre modifie",
            Contenu: "Contenu modifie",
            ModuleConcerne: "Dashboard",
            StatutSuggestion: "Acceptee",
            CommentaireAdmin: "Bonne idee",
            UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Suggestion>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateAsync(
            It.Is<Suggestion>(sg =>
                sg.Id == 1 &&
                sg.Name == "Suggestion modifiee" &&
                sg.StatutSuggestion == "Acceptee" &&
                sg.CommentaireAdmin == "Bonne idee"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateCommand_RetourneFalse_SiInexistante()
    {
        var command = new UpdateSuggestionCommand(
            Id: 999, Name: "Test", Description: null, TypeId: 1, AuteurId: 1,
            Titre: "Test", Contenu: "Test", ModuleConcerne: null,
            StatutSuggestion: "Soumise", CommentaireAdmin: null, UserId: 1);

        _serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Suggestion>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Delete ───

    [Fact]
    public async Task Handle_DeleteCommand_AppelleService()
    {
        var command = new DeleteSuggestionCommand(1);

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
        var command = new DeleteSuggestionCommand(999);

        _serviceMock
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── Voter ───

    [Fact]
    public async Task Handle_VoterCommand_AppelleService_EtRetourneTrue()
    {
        var command = new VoterSuggestionCommand(SuggestionId: 1, VotantId: 10, UserId: 1);

        _serviceMock
            .Setup(s => s.VoterAsync(1, 10, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.VoterAsync(1, 10, 1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_VoterCommand_RetourneFalse_SiDejaVote()
    {
        var command = new VoterSuggestionCommand(SuggestionId: 1, VotantId: 10, UserId: 1);

        _serviceMock
            .Setup(s => s.VoterAsync(1, 10, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    // ─── RetirerVote ───

    [Fact]
    public async Task Handle_RetirerVoteCommand_AppelleService_EtRetourneTrue()
    {
        var command = new RetirerVoteSuggestionCommand(SuggestionId: 1, VotantId: 10);

        _serviceMock
            .Setup(s => s.RetirerVoteAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _serviceMock.Verify(s => s.RetirerVoteAsync(1, 10, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RetirerVoteCommand_RetourneFalse_SiVoteInexistant()
    {
        var command = new RetirerVoteSuggestionCommand(SuggestionId: 999, VotantId: 10);

        _serviceMock
            .Setup(s => s.RetirerVoteAsync(999, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
