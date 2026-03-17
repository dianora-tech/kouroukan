using Bde.Domain.Entities;
using Bde.Infrastructure.Dtos;

namespace Bde.Infrastructure.Mappers;

public static class DepenseBdeMapper
{
    public static DepenseBde ToEntity(DepenseBdeDto dto)
    {
        return new DepenseBde
        {
            Id = dto.Id,
            TypeId = dto.TypeId,
            Name = dto.Name,
            Description = dto.Description,
            AssociationId = dto.AssociationId,
            Montant = dto.Montant,
            Motif = dto.Motif,
            Categorie = dto.Categorie,
            StatutValidation = dto.StatutValidation,
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

    public static DepenseBdeDto ToDto(DepenseBde entity)
    {
        return new DepenseBdeDto
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            Name = entity.Name,
            Description = entity.Description,
            AssociationId = entity.AssociationId,
            Montant = entity.Montant,
            Motif = entity.Motif,
            Categorie = entity.Categorie,
            StatutValidation = entity.StatutValidation,
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
