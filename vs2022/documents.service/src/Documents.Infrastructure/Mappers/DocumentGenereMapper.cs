using Documents.Domain.Entities;
using Documents.Infrastructure.Dtos;

namespace Documents.Infrastructure.Mappers;

public static class DocumentGenereMapper
{
    public static DocumentGenere ToEntity(DocumentGenereDto dto)
    {
        return new DocumentGenere
        {
            Id = dto.Id,
            TypeId = dto.TypeId,
            ModeleDocumentId = dto.ModeleDocumentId,
            EleveId = dto.EleveId,
            EnseignantId = dto.EnseignantId,
            DonneesJson = dto.DonneesJson,
            DateGeneration = dto.DateGeneration,
            StatutSignature = dto.StatutSignature,
            CheminFichier = dto.CheminFichier,
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

    public static DocumentGenereDto ToDto(DocumentGenere entity)
    {
        return new DocumentGenereDto
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            ModeleDocumentId = entity.ModeleDocumentId,
            EleveId = entity.EleveId,
            EnseignantId = entity.EnseignantId,
            DonneesJson = entity.DonneesJson,
            DateGeneration = entity.DateGeneration,
            StatutSignature = entity.StatutSignature,
            CheminFichier = entity.CheminFichier,
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
