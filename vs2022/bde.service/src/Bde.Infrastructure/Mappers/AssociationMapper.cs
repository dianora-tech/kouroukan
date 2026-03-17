using Bde.Domain.Entities;
using Bde.Infrastructure.Dtos;

namespace Bde.Infrastructure.Mappers;

public static class AssociationMapper
{
    public static Association ToEntity(AssociationDto dto)
    {
        return new Association
        {
            Id = dto.Id,
            TypeId = dto.TypeId,
            Name = dto.Name,
            Description = dto.Description,
            Sigle = dto.Sigle,
            AnneeScolaire = dto.AnneeScolaire,
            Statut = dto.Statut,
            BudgetAnnuel = dto.BudgetAnnuel,
            SuperviseurId = dto.SuperviseurId,
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

    public static AssociationDto ToDto(Association entity)
    {
        return new AssociationDto
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            Name = entity.Name,
            Description = entity.Description,
            Sigle = entity.Sigle,
            AnneeScolaire = entity.AnneeScolaire,
            Statut = entity.Statut,
            BudgetAnnuel = entity.BudgetAnnuel,
            SuperviseurId = entity.SuperviseurId,
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
