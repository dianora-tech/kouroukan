using Finances.Domain.Entities;
using Finances.Infrastructure.Dtos;

namespace Finances.Infrastructure.Mappers;

/// <summary>
/// Mapper bidirectionnel Facture ↔ FactureDto.
/// </summary>
public static class FactureMapper
{
    public static Facture ToEntity(FactureDto dto)
    {
        return new Facture
        {
            Id = dto.Id,
            TypeId = dto.TypeId,
            EleveId = dto.EleveId,
            ParentId = dto.ParentId,
            AnneeScolaireId = dto.AnneeScolaireId,
            MontantTotal = dto.MontantTotal,
            MontantPaye = dto.MontantPaye,
            Solde = dto.Solde,
            DateEmission = dto.DateEmission,
            DateEcheance = dto.DateEcheance,
            StatutFacture = dto.StatutFacture,
            NumeroFacture = dto.NumeroFacture,
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

    public static FactureDto ToDto(Facture entity)
    {
        return new FactureDto
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            EleveId = entity.EleveId,
            ParentId = entity.ParentId,
            AnneeScolaireId = entity.AnneeScolaireId,
            MontantTotal = entity.MontantTotal,
            MontantPaye = entity.MontantPaye,
            Solde = entity.Solde,
            DateEmission = entity.DateEmission,
            DateEcheance = entity.DateEcheance,
            StatutFacture = entity.StatutFacture,
            NumeroFacture = entity.NumeroFacture,
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
