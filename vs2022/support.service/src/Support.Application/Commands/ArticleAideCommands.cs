using MediatR;
using Support.Domain.Entities;

namespace Support.Application.Commands;

public sealed record CreateArticleAideCommand(
    string Name,
    string? Description,
    int TypeId,
    string Titre,
    string Contenu,
    string Slug,
    string Categorie,
    string? ModuleConcerne,
    int Ordre,
    bool EstPublie,
    int UserId) : IRequest<ArticleAide>;

public sealed record UpdateArticleAideCommand(
    int Id,
    string Name,
    string? Description,
    int TypeId,
    string Titre,
    string Contenu,
    string Slug,
    string Categorie,
    string? ModuleConcerne,
    int Ordre,
    bool EstPublie,
    int UserId) : IRequest<bool>;

public sealed record DeleteArticleAideCommand(int Id) : IRequest<bool>;

public sealed record MarquerArticleUtileCommand(int Id) : IRequest<bool>;
