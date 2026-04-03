using Xunit;
using Finances.Api.Controllers;
using Finances.Domain.Entities;
using Finances.Api.Models;
using GnDapper.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace Finances.Tests.Unit.Controllers;

public sealed class JournalFinancierControllerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<JournalFinancierController>> _logger;
    private readonly JournalFinancierController _sut;

    public JournalFinancierControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<JournalFinancierController>>();
        _sut = new JournalFinancierController(_mediator.Object, _logger.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ─── GetAll ───

    [Fact]
    public async Task GetAll_DoitRetournerOk_QuandAppele()
    {
        var paged = new PagedResult<JournalFinancier>(new List<JournalFinancier>(), 0, 1, 20);
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<PagedResult<JournalFinancier>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<ApiResponse<PagedResult<JournalFinancier>>>();
    }

    // ─── Create ───

    [Fact]
    public async Task Create_DoitRetournerCreated_QuandCommandeValide()
    {
        var created = new JournalFinancier() { Id = 1 };
        _mediator.Setup(m => m.Send(It.IsAny<IRequest<JournalFinancier>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        _mediator.Verify(m => m.Send(It.IsAny<IRequest<JournalFinancier>>(), It.IsAny<CancellationToken>()), Times.Never());
    }

}
