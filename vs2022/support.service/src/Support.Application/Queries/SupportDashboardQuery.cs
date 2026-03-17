using MediatR;

namespace Support.Application.Queries;

/// <summary>
/// Donnees du dashboard support.
/// </summary>
public sealed record SupportDashboardResult
{
    public int TicketsOuverts { get; init; }
    public int TicketsEnCours { get; init; }
    public int TicketsEnAttente { get; init; }
    public int TicketsResolus { get; init; }
    public int TicketsFermes { get; init; }
    public double TempsMoyenResolutionHeures { get; init; }
    public double TauxSatisfactionMoyen { get; init; }
    public IReadOnlyList<SuggestionResume> TopSuggestions { get; init; } = [];
    public IReadOnlyList<ArticleResume> TopArticles { get; init; } = [];
    public int ConversationsIAActives { get; init; }
    public long TokensConsommes { get; init; }
}

public sealed record SuggestionResume(int Id, string Titre, int NombreVotes);
public sealed record ArticleResume(int Id, string Titre, int NombreVues);

public sealed record GetSupportDashboardQuery() : IRequest<SupportDashboardResult>;
