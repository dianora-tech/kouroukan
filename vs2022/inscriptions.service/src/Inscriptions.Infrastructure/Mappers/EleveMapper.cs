using Inscriptions.Domain.Entities;
using Inscriptions.Infrastructure.Dtos;

namespace Inscriptions.Infrastructure.Mappers;

public static class EleveMapper
{
    public static Eleve ToEntity(EleveDto dto)
    {
        return new Eleve
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateNaissance = dto.DateNaissance,
            LieuNaissance = dto.LieuNaissance,
            Genre = dto.Genre,
            Nationalite = dto.Nationalite,
            Adresse = dto.Adresse,
            PhotoUrl = dto.PhotoUrl,
            NumeroMatricule = dto.NumeroMatricule,
            NiveauClasseId = dto.NiveauClasseId,
            ClasseId = dto.ClasseId,
            ParentId = dto.ParentId,
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

    public static EleveDto ToDto(Eleve entity)
    {
        return new EleveDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            DateNaissance = entity.DateNaissance,
            LieuNaissance = entity.LieuNaissance,
            Genre = entity.Genre,
            Nationalite = entity.Nationalite,
            Adresse = entity.Adresse,
            PhotoUrl = entity.PhotoUrl,
            NumeroMatricule = entity.NumeroMatricule,
            NiveauClasseId = entity.NiveauClasseId,
            ClasseId = entity.ClasseId,
            ParentId = entity.ParentId,
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
