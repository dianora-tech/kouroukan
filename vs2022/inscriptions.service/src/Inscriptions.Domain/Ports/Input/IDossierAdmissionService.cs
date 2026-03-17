using GnDapper.Models;
using Inscriptions.Domain.Entities;

namespace Inscriptions.Domain.Ports.Input;

/// <summary>
/// Port d'entree pour la logique metier des dossiers d'admission.
/// </summary>
public interface IDossierAdmissionService
{
    Task<DossierAdmission?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<DossierAdmission>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<DossierAdmission>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<DossierAdmission> CreateAsync(DossierAdmission entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(DossierAdmission entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
