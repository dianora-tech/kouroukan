using Finances.Domain.Entities;
using Finances.Infrastructure.Dtos;

namespace Finances.Infrastructure.Mappers;

public static class MoyenPaiementMapper
{
    public static MoyenPaiement ToEntity(MoyenPaiementDto dto)
    {
        return new MoyenPaiement
        {
            Id = dto.Id,
            CompanyId = dto.CompanyId,
            Operateur = dto.Operateur,
            NumeroCompte = dto.NumeroCompte,
            CodeMarchand = dto.CodeMarchand,
            Libelle = dto.Libelle,
            EstActif = dto.EstActif,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CreatedBy = dto.CreatedBy,
            UpdatedBy = dto.UpdatedBy,
            IsDeleted = dto.IsDeleted,
            DeletedAt = dto.DeletedAt,
            DeletedBy = dto.DeletedBy,
        };
    }

    public static MoyenPaiementDto ToDto(MoyenPaiement entity)
    {
        return new MoyenPaiementDto
        {
            Id = entity.Id,
            CompanyId = entity.CompanyId,
            Operateur = entity.Operateur,
            NumeroCompte = entity.NumeroCompte,
            CodeMarchand = entity.CodeMarchand,
            Libelle = entity.Libelle,
            EstActif = entity.EstActif,
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
