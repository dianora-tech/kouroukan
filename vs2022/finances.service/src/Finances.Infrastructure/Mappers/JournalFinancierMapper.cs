using Finances.Domain.Entities;
using Finances.Infrastructure.Dtos;

namespace Finances.Infrastructure.Mappers;

public static class JournalFinancierMapper
{
    public static JournalFinancier ToEntity(JournalFinancierDto dto)
    {
        return new JournalFinancier
        {
            Id = dto.Id,
            CompanyId = dto.CompanyId,
            Type = dto.Type,
            Categorie = dto.Categorie,
            Montant = dto.Montant,
            Description = dto.Description,
            ReferenceExterne = dto.ReferenceExterne,
            ModePaiement = dto.ModePaiement,
            DateOperation = dto.DateOperation,
            EleveId = dto.EleveId,
            ParentUserId = dto.ParentUserId,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CreatedBy = dto.CreatedBy,
            UpdatedBy = dto.UpdatedBy,
            IsDeleted = dto.IsDeleted,
            DeletedAt = dto.DeletedAt,
            DeletedBy = dto.DeletedBy,
        };
    }

    public static JournalFinancierDto ToDto(JournalFinancier entity)
    {
        return new JournalFinancierDto
        {
            Id = entity.Id,
            CompanyId = entity.CompanyId,
            Type = entity.Type,
            Categorie = entity.Categorie,
            Montant = entity.Montant,
            Description = entity.Description,
            ReferenceExterne = entity.ReferenceExterne,
            ModePaiement = entity.ModePaiement,
            DateOperation = entity.DateOperation,
            EleveId = entity.EleveId,
            ParentUserId = entity.ParentUserId,
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
