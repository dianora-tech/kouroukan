using GnDapper.Models;
using Inscriptions.Domain.Entities;

namespace Inscriptions.Domain.Ports.Output;

/// <summary>
/// Port de sortie pour la persistance des dossiers d'admission.
/// </summary>
public interface IDossierAdmissionRepository
{
    Task<DossierAdmission?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<DossierAdmission>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<DossierAdmission>> GetPagedAsync(int page, int pageSize, string? search, int? typeId, string? orderBy, CancellationToken ct = default);
    Task<DossierAdmission> AddAsync(DossierAdmission entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(DossierAdmission entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TypeDto>> GetTypesAsync(CancellationToken ct = default);
}

/// <summary>
/// DTO generique pour les tables de types (id, name, description).
/// </summary>
public sealed record TypeDto(int Id, string Name, string? Description);
