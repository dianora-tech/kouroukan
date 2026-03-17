using Personnel.Domain.Entities;
using Personnel.Infrastructure.Dtos;

namespace Personnel.Infrastructure.Mappers;

public static class DemandeCongeMapper
{
    public static DemandeConge ToEntity(DemandeCongeDto dto)
    {
        return new DemandeConge
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            EnseignantId = dto.EnseignantId,
            DateDebut = dto.DateDebut,
            DateFin = dto.DateFin,
            Motif = dto.Motif,
            StatutDemande = dto.StatutDemande,
            PieceJointeUrl = dto.PieceJointeUrl,
            CommentaireValidateur = dto.CommentaireValidateur,
            ValidateurId = dto.ValidateurId,
            DateValidation = dto.DateValidation,
            ImpactPaie = dto.ImpactPaie,
            TypeId = dto.TypeId,
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

    public static DemandeCongeDto ToDto(DemandeConge entity)
    {
        return new DemandeCongeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            EnseignantId = entity.EnseignantId,
            DateDebut = entity.DateDebut,
            DateFin = entity.DateFin,
            Motif = entity.Motif,
            StatutDemande = entity.StatutDemande,
            PieceJointeUrl = entity.PieceJointeUrl,
            CommentaireValidateur = entity.CommentaireValidateur,
            ValidateurId = entity.ValidateurId,
            DateValidation = entity.DateValidation,
            ImpactPaie = entity.ImpactPaie,
            TypeId = entity.TypeId,
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
