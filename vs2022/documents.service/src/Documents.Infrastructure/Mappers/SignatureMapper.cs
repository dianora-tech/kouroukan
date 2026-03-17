using Documents.Domain.Entities;
using Documents.Infrastructure.Dtos;

namespace Documents.Infrastructure.Mappers;

public static class SignatureMapper
{
    public static Signature ToEntity(SignatureDto dto)
    {
        return new Signature
        {
            Id = dto.Id,
            TypeId = dto.TypeId,
            DocumentGenereId = dto.DocumentGenereId,
            SignataireId = dto.SignataireId,
            OrdreSignature = dto.OrdreSignature,
            DateSignature = dto.DateSignature,
            StatutSignature = dto.StatutSignature,
            NiveauSignature = dto.NiveauSignature,
            MotifRefus = dto.MotifRefus,
            EstValidee = dto.EstValidee,
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

    public static SignatureDto ToDto(Signature entity)
    {
        return new SignatureDto
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            DocumentGenereId = entity.DocumentGenereId,
            SignataireId = entity.SignataireId,
            OrdreSignature = entity.OrdreSignature,
            DateSignature = entity.DateSignature,
            StatutSignature = entity.StatutSignature,
            NiveauSignature = entity.NiveauSignature,
            MotifRefus = entity.MotifRefus,
            EstValidee = entity.EstValidee,
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
