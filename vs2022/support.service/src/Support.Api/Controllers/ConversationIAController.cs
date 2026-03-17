using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Support.Api.Models;
using Support.Application.Commands;
using Support.Application.Queries;
using Support.Domain.Entities;

namespace Support.Api.Controllers;

/// <summary>
/// Controleur pour les conversations d'aide generative IA.
/// </summary>
[ApiController]
[Route("api/support/conversations-ia")]
[Authorize]
public sealed class ConversationIAController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ConversationIAController> _logger;

    public ConversationIAController(IMediator mediator, ILogger<ConversationIAController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Demarre une nouvelle conversation IA.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:support:create")]
    public async Task<ActionResult<ApiResponse<ConversationIA>>> Create(
        [FromBody] CreerConversationIACommand command, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("POST conversations-ia pour utilisateur {UtilisateurId} [CorrelationId: {CorrelationId}]",
            command.UtilisateurId, correlationId);

        var created = await _mediator.Send(command, ct);
        var response = ApiResponse<ConversationIA>.Ok(created, "Conversation IA demarree.");
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Envoie un message et obtient la reponse IA.
    /// </summary>
    [HttpPost("{id:int}/messages")]
    [Authorize(Policy = "RequirePermission:support:create")]
    public async Task<ActionResult<ApiResponse<string>>> EnvoyerMessage(
        int id, [FromBody] EnvoyerMessageRequest request, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("POST message pour conversation {ConversationId} [CorrelationId: {CorrelationId}]",
            id, correlationId);

        var command = new EnvoyerMessageIACommand(id, request.Question, request.UserId);
        var reponse = await _mediator.Send(command, ct);
        var response = ApiResponse<string>.Ok(reponse);
        response.CorrelationId = correlationId;
        return Ok(response);
    }

    /// <summary>
    /// Recupere l'historique des messages d'une conversation.
    /// </summary>
    [HttpGet("{id:int}/messages")]
    [Authorize(Policy = "RequirePermission:support:read")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MessageIA>>>> GetMessages(int id, CancellationToken ct)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var messages = await _mediator.Send(new GetMessagesConversationIAQuery(id), ct);
        var response = ApiResponse<IReadOnlyList<MessageIA>>.Ok(messages);
        response.CorrelationId = correlationId;
        return Ok(response);
    }
}

/// <summary>
/// Requete pour envoyer un message IA.
/// </summary>
public sealed record EnvoyerMessageRequest(string Question, int UserId);
