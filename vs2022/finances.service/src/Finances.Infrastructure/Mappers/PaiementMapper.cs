using Finances.Domain.Entities;
using Finances.Infrastructure.Dtos;

namespace Finances.Infrastructure.Mappers;

/// <summary>
/// Mapper bidirectionnel Paiement ↔ PaiementDto.
/// </summary>
public static class PaiementMapper
{
    public static Paiement ToEntity(PaiementDto dto)
    {
        return new Paiement
        {
            Id = dto.Id,
            TypeId = dto.TypeId,
            FactureId = dto.FactureId,
            MontantPaye = dto.MontantPaye,
            DatePaiement = dto.DatePaiement,
            MoyenPaiement = dto.MoyenPaiement,
            ReferenceMobileMoney = dto.ReferenceMobileMoney,
            StatutPaiement = dto.StatutPaiement,
            CaissierId = dto.CaissierId,
            NumeroRecu = dto.NumeroRecu,
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

    public static PaiementDto ToDto(Paiement entity)
    {
        return new PaiementDto
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            FactureId = entity.FactureId,
            MontantPaye = entity.MontantPaye,
            DatePaiement = entity.DatePaiement,
            MoyenPaiement = entity.MoyenPaiement,
            ReferenceMobileMoney = entity.ReferenceMobileMoney,
            StatutPaiement = entity.StatutPaiement,
            CaissierId = entity.CaissierId,
            NumeroRecu = entity.NumeroRecu,
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
