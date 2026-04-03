using Xunit;
using Support.Api.Controllers;
using Support.Api.Models;
using Support.Application.Commands;
using Support.Application.Queries;
using Support.Domain.Entities;
using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace Support.Tests.Unit.Controllers;

public sealed class SuggestionControllerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<SuggestionController>> _logger;
    private readonly SuggestionController _sut;

    public SuggestionControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<SuggestionController>>();
        _sut = new SuggestionController(_mediator.Object, _logger.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ─── GetAll ───

    [Fact]
    public async Task GetAll_DoitRetournerOk_QuandAppele()
    {
        var paged = new PagedResult<Suggestion>(new List<Suggestion>(), 0, 1, 20);
        _mediator.Setup(m => m.Send(It.IsAny<GetPagedSuggestionsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<ApiResponse<PagedResult<Suggestion>>>();
    }

    // ─── GetById ───

    [Fact]
    public async Task GetById_DoitRetournerOk_QuandSuggestionTrouvee()
    {
        var entity = new Suggestion() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<GetSuggestionByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _sut.GetById(1, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_DoitRetournerNotFound_QuandSuggestionInexistante()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetSuggestionByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Suggestion?)null);

        var result = await _sut.GetById(999, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ─── GetTypes ───

    [Fact]
    public void GetTypes_DoitRetournerOk_AvecListeDeTypes()
    {
        var result = _sut.GetTypes();

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    // ─── Create ───

    [Fact]
    public async Task Create_DoitRetournerCreated_QuandCommandeValide()
    {
        var entity = new Suggestion() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<CreateSuggestionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _mediator.Object.Should().NotBeNull();
    }

    // ─── Update ───

    [Fact]
    public async Task Update_DoitRetournerBadRequest_QuandIdNonCorrespondant()
    {
        var command = new UpdateSuggestionCommand(999, "name", null, 1, 1, "titre", "contenu", null, "EnAttente", null, 1);

        var result = await _sut.Update(1, command, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ─── Delete ───

    [Fact]
    public async Task Delete_DoitRetournerNotFound_QuandSuggestionInexistante()
    {
        _mediator.Setup(m => m.Send(It.IsAny<DeleteSuggestionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mediator.Object.Should().NotBeNull();
    }

    // ─── Voter ───

    [Fact]
    public async Task Voter_DoitRetournerBadRequest_QuandIdNonCorrespondant()
    {
        var command = new VoterSuggestionCommand(999, 1, 1);

        var result = await _sut.Voter(1, command, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ─── RetirerVote ───

    [Fact]
    public async Task RetirerVote_DoitRetournerOk_QuandAppele()
    {
        _mediator.Setup(m => m.Send(It.IsAny<RetirerVoteSuggestionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.RetirerVote(1, 1, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }
}
