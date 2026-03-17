using Finances.Domain.Entities;
using Finances.Infrastructure.Dtos;

namespace Finances.Infrastructure.Mappers;

/// <summary>
/// Mapper bidirectionnel RemunerationEnseignant ↔ RemunerationEnseignantDto.
/// </summary>
public static class RemunerationEnseignantMapper
{
    public static RemunerationEnseignant ToEntity(RemunerationEnseignantDto dto)
    {
        return new RemunerationEnseignant
        {
            Id = dto.Id,
            EnseignantId = dto.EnseignantId,
            Mois = dto.Mois,
            Annee = dto.Annee,
            ModeRemuneration = dto.ModeRemuneration,
            MontantForfait = dto.MontantForfait,
            NombreHeures = dto.NombreHeures,
            TauxHoraire = dto.TauxHoraire,
            MontantTotal = dto.MontantTotal,
            StatutPaiement = dto.StatutPaiement,
            DateValidation = dto.DateValidation,
            ValidateurId = dto.ValidateurId,
            UserId = dto.UserId,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CreatedBy = dto.CreatedBy,
            UpdatedBy = dto.UpdatedBy,
            IsDeleted = dto.IsDeleted,
            DeletedAt = dto.DeletedAt,
            DeletedBy = dto.DeletedBy,
        };
    }

    public static RemunerationEnseignantDto ToDto(RemunerationEnseignant entity)
    {
        return new RemunerationEnseignantDto
        {
            Id = entity.Id,
            EnseignantId = entity.EnseignantId,
            Mois = entity.Mois,
            Annee = entity.Annee,
            ModeRemuneration = entity.ModeRemuneration,
            MontantForfait = entity.MontantForfait,
            NombreHeures = entity.NombreHeures,
            TauxHoraire = entity.TauxHoraire,
            MontantTotal = entity.MontantTotal,
            StatutPaiement = entity.StatutPaiement,
            DateValidation = entity.DateValidation,
            ValidateurId = entity.ValidateurId,
            UserId = entity.UserId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedBy = entity.UpdatedBy,
            IsDeleted = entity.IsDeleted,
            DeletedAt = entity.DeletedAt,
            DeletedBy = entity.DeletedBy,
        };
    }
}
