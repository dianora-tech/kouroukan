using Inscriptions.Domain.Entities;
using Inscriptions.Infrastructure.Dtos;

namespace Inscriptions.Infrastructure.Mappers;

public static class DossierAdmissionMapper
{
    public static DossierAdmission ToEntity(DossierAdmissionDto dto)
    {
        return new DossierAdmission
        {
            Id = dto.Id,
            TypeId = dto.TypeId,
            EleveId = dto.EleveId,
            AnneeScolaireId = dto.AnneeScolaireId,
            StatutDossier = dto.StatutDossier,
            EtapeActuelle = dto.EtapeActuelle,
            DateDemande = dto.DateDemande,
            DateDecision = dto.DateDecision,
            MotifRefus = dto.MotifRefus,
            ScoringInterne = dto.ScoringInterne,
            Commentaires = dto.Commentaires,
            ResponsableAdmissionId = dto.ResponsableAdmissionId,
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

    public static DossierAdmissionDto ToDto(DossierAdmission entity)
    {
        return new DossierAdmissionDto
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            EleveId = entity.EleveId,
            AnneeScolaireId = entity.AnneeScolaireId,
            StatutDossier = entity.StatutDossier,
            EtapeActuelle = entity.EtapeActuelle,
            DateDemande = entity.DateDemande,
            DateDecision = entity.DateDecision,
            MotifRefus = entity.MotifRefus,
            ScoringInterne = entity.ScoringInterne,
            Commentaires = entity.Commentaires,
            ResponsableAdmissionId = entity.ResponsableAdmissionId,
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
