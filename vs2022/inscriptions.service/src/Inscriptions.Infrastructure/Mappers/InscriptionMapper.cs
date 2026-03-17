using Inscriptions.Domain.Entities;
using Inscriptions.Infrastructure.Dtos;

namespace Inscriptions.Infrastructure.Mappers;

public static class InscriptionMapper
{
    public static Inscription ToEntity(InscriptionDto dto)
    {
        return new Inscription
        {
            Id = dto.Id,
            TypeId = dto.TypeId,
            EleveId = dto.EleveId,
            ClasseId = dto.ClasseId,
            AnneeScolaireId = dto.AnneeScolaireId,
            DateInscription = dto.DateInscription,
            MontantInscription = dto.MontantInscription,
            EstPaye = dto.EstPaye,
            EstRedoublant = dto.EstRedoublant,
            TypeEtablissement = dto.TypeEtablissement,
            SerieBac = dto.SerieBac,
            StatutInscription = dto.StatutInscription,
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

    public static InscriptionDto ToDto(Inscription entity)
    {
        return new InscriptionDto
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            EleveId = entity.EleveId,
            ClasseId = entity.ClasseId,
            AnneeScolaireId = entity.AnneeScolaireId,
            DateInscription = entity.DateInscription,
            MontantInscription = entity.MontantInscription,
            EstPaye = entity.EstPaye,
            EstRedoublant = entity.EstRedoublant,
            TypeEtablissement = entity.TypeEtablissement,
            SerieBac = entity.SerieBac,
            StatutInscription = entity.StatutInscription,
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
