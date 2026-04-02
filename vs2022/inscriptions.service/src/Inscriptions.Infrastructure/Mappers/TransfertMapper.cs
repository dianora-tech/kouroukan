using Inscriptions.Domain.Entities;
using Inscriptions.Infrastructure.Dtos;

namespace Inscriptions.Infrastructure.Mappers;

public static class TransfertMapper
{
    public static Transfert ToEntity(TransfertDto dto)
    {
        return new Transfert
        {
            Id = dto.Id,
            EleveId = dto.EleveId,
            CompanyOrigineId = dto.CompanyOrigineId,
            CompanyCibleId = dto.CompanyCibleId,
            Statut = dto.Statut,
            Motif = dto.Motif,
            Documents = dto.Documents,
            DateDemande = dto.DateDemande,
            DateTraitement = dto.DateTraitement,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            CreatedBy = dto.CreatedBy,
            UpdatedBy = dto.UpdatedBy,
            IsDeleted = dto.IsDeleted,
            DeletedAt = dto.DeletedAt,
            DeletedBy = dto.DeletedBy,
        };
    }

    public static TransfertDto ToDto(Transfert entity)
    {
        return new TransfertDto
        {
            Id = entity.Id,
            EleveId = entity.EleveId,
            CompanyOrigineId = entity.CompanyOrigineId,
            CompanyCibleId = entity.CompanyCibleId,
            Statut = entity.Statut,
            Motif = entity.Motif,
            Documents = entity.Documents,
            DateDemande = entity.DateDemande,
            DateTraitement = entity.DateTraitement,
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
