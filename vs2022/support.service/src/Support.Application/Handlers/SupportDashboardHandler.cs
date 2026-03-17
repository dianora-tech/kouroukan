using MediatR;
using Support.Application.Queries;
using Support.Domain.Ports.Output;

namespace Support.Application.Handlers;

/// <summary>
/// Handler pour le dashboard support.
/// </summary>
public sealed class SupportDashboardHandler : IRequestHandler<GetSupportDashboardQuery, SupportDashboardResult>
{
    private readonly ITicketRepository _ticketRepo;
    private readonly ISuggestionRepository _suggestionRepo;
    private readonly IArticleAideRepository _articleRepo;
    private readonly IConversationIARepository _conversationRepo;

    public SupportDashboardHandler(
        ITicketRepository ticketRepo,
        ISuggestionRepository suggestionRepo,
        IArticleAideRepository articleRepo,
        IConversationIARepository conversationRepo)
    {
        _ticketRepo = ticketRepo;
        _suggestionRepo = suggestionRepo;
        _articleRepo = articleRepo;
        _conversationRepo = conversationRepo;
    }

    public async Task<SupportDashboardResult> Handle(GetSupportDashboardQuery request, CancellationToken ct)
    {
        var ouverts = await _ticketRepo.CountByStatutAsync("Ouvert", ct);
        var enCours = await _ticketRepo.CountByStatutAsync("EnCours", ct);
        var enAttente = await _ticketRepo.CountByStatutAsync("EnAttente", ct);
        var resolus = await _ticketRepo.CountByStatutAsync("Resolu", ct);
        var fermes = await _ticketRepo.CountByStatutAsync("Ferme", ct);
        var tempsResolution = await _ticketRepo.GetTempsMoyenResolutionAsync(ct);
        var satisfaction = await _ticketRepo.GetMoyenneSatisfactionAsync(ct);

        var topSuggestions = await _suggestionRepo.GetTopVoteesAsync(10, ct);
        var topArticles = await _articleRepo.GetTopConsultesAsync(10, ct);
        var conversationsActives = await _conversationRepo.GetTotalConversationsActivesAsync(ct);
        var tokensConsommes = await _conversationRepo.GetTotalTokensConsommesAsync(ct);

        return new SupportDashboardResult
        {
            TicketsOuverts = ouverts,
            TicketsEnCours = enCours,
            TicketsEnAttente = enAttente,
            TicketsResolus = resolus,
            TicketsFermes = fermes,
            TempsMoyenResolutionHeures = tempsResolution,
            TauxSatisfactionMoyen = satisfaction,
            TopSuggestions = topSuggestions.Select(s => new SuggestionResume(s.Id, s.Titre, s.NombreVotes)).ToList(),
            TopArticles = topArticles.Select(a => new ArticleResume(a.Id, a.Titre, a.NombreVues)).ToList(),
            ConversationsIAActives = conversationsActives,
            TokensConsommes = tokensConsommes
        };
    }
}
