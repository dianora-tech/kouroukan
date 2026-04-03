using Evaluations.Domain.Entities;
using Evaluations.Infrastructure.Dtos;

namespace Evaluations.Infrastructure.Mappers;

public static class BulletinMapper
{
    public static Bulletin ToEntity(BulletinDto dto)
    {
        return new Bulletin
        {
            Id = dto.Id,
            EleveId = dto.EleveId,
            ClasseId = dto.ClasseId,
            Trimestre = dto.Trimestre,
            AnneeScolaireId = dto.AnneeScolaireId,
            MoyenneGenerale = dto.MoyenneGenerale,
            Rang = dto.Rang,
            Appreciation = dto.Appreciation,
            EstPublie = dto.EstPublie,
            DateGeneration = dto.DateGeneration,
            CheminFichierPdf = dto.CheminFichierPdf,
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

    public static BulletinDto ToDto(Bulletin entity)
    {
        return new BulletinDto
        {
            Id = entity.Id,
            EleveId = entity.EleveId,
            ClasseId = entity.ClasseId,
            Trimestre = entity.Trimestre,
            AnneeScolaireId = entity.AnneeScolaireId,
            MoyenneGenerale = entity.MoyenneGenerale,
            Rang = entity.Rang,
            Appreciation = entity.Appreciation,
            EstPublie = entity.EstPublie,
            DateGeneration = entity.DateGeneration,
            CheminFichierPdf = entity.CheminFichierPdf,
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
