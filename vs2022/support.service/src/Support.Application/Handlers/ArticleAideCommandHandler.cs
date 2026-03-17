using MediatR;
using Support.Application.Commands;
using Support.Domain.Entities;
using Support.Domain.Ports.Input;

namespace Support.Application.Handlers;

/// <summary>
/// Handler pour les commandes d'articles d'aide.
/// </summary>
public sealed class ArticleAideCommandHandler :
    IRequestHandler<CreateArticleAideCommand, ArticleAide>,
    IRequestHandler<UpdateArticleAideCommand, bool>,
    IRequestHandler<DeleteArticleAideCommand, bool>,
    IRequestHandler<MarquerArticleUtileCommand, bool>
{
    private readonly IArticleAideService _service;

    public ArticleAideCommandHandler(IArticleAideService service) => _service = service;

    public async Task<ArticleAide> Handle(CreateArticleAideCommand request, CancellationToken ct)
    {
        var entity = new ArticleAide
        {
            Name = request.Name,
            Description = request.Description,
            TypeId = request.TypeId,
            Titre = request.Titre,
            Contenu = request.Contenu,
            Slug = request.Slug,
            Categorie = request.Categorie,
            ModuleConcerne = request.ModuleConcerne,
            Ordre = request.Ordre,
            EstPublie = request.EstPublie,
            UserId = request.UserId
        };
        return await _service.CreateAsync(entity, ct);
    }

    public async Task<bool> Handle(UpdateArticleAideCommand request, CancellationToken ct)
    {
        var entity = new ArticleAide
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            TypeId = request.TypeId,
            Titre = request.Titre,
            Contenu = request.Contenu,
            Slug = request.Slug,
            Categorie = request.Categorie,
            ModuleConcerne = request.ModuleConcerne,
            Ordre = request.Ordre,
            EstPublie = request.EstPublie,
            UserId = request.UserId
        };
        return await _service.UpdateAsync(entity, ct);
    }

    public async Task<bool> Handle(DeleteArticleAideCommand request, CancellationToken ct) =>
        await _service.DeleteAsync(request.Id, ct);

    public async Task<bool> Handle(MarquerArticleUtileCommand request, CancellationToken ct) =>
        await _service.MarquerUtileAsync(request.Id, ct);
}
