namespace GnValidation.Commands.Support;

/// <summary>Commande de creation d'un article d'aide.</summary>
public record CreateArticleAideCommand(
    string Titre,
    string Contenu,
    string Slug,
    string Categorie,
    string? ModuleConcerne,
    int Ordre,
    bool EstPublie);
