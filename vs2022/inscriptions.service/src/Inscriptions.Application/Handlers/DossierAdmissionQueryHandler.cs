using GnDapper.Models;
using Inscriptions.Application.Queries;
using Inscriptions.Domain.Entities;
using Inscriptions.Domain.Ports.Input;
using MediatR;

namespace Inscriptions.Application.Handlers;

/// <summary>
/// Gestionnaire des requetes pour les dossiers d'admission.
/// </summary>
public sealed class DossierAdmissionQueryHandler :
    IRequestHandler<GetDossierAdmissionByIdQuery, DossierAdmission?>,
    IRequestHandler<GetAllDossierAdmissionsQuery, IReadOnlyList<DossierAdmission>>,
    IRequestHandler<GetPagedDossierAdmissionsQuery, PagedResult<DossierAdmission>>
{
    private readonly IDossierAdmissionService _service;

    public DossierAdmissionQueryHandler(IDossierAdmissionService service)
    {
        _service = service;
    }

    public async Task<DossierAdmission?> Handle(GetDossierAdmissionByIdQuery request, CancellationToken ct)
    {
        return await _service.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<DossierAdmission>> Handle(GetAllDossierAdmissionsQuery request, CancellationToken ct)
    {
        return await _service.GetAllAsync(ct).ConfigureAwait(false);
    }

    public async Task<PagedResult<DossierAdmission>> Handle(GetPagedDossierAdmissionsQuery request, CancellationToken ct)
    {
        return await _service.GetPagedAsync(request.Page, request.PageSize, request.Search, request.TypeId, request.OrderBy, ct).ConfigureAwait(false);
    }
}
