using Finances.Domain.Entities;
using Finances.Infrastructure.Dtos;

namespace Finances.Infrastructure.Mappers;

/// <summary>
/// Mapper bidirectionnel Depense ↔ DepenseDto.
/// </summary>
public static class DepenseMapper
{
    public static Depense ToEntity(DepenseDto dto)
    {
        return new Depense
        {
            Id = dto.Id,
            TypeId = dto.TypeId,
            Montant = dto.Montant,
            MotifDepense = dto.MotifDepense,
            Categorie = dto.Categorie,
            BeneficiaireNom = dto.BeneficiaireNom,
            BeneficiaireTelephone = dto.BeneficiaireTelephone,
            BeneficiaireNIF = dto.BeneficiaireNIF,
            StatutDepense = dto.StatutDepense,
            DateDemande = dto.DateDemande,
            DateValidation = dto.DateValidation,
            ValidateurId = dto.ValidateurId,
            PieceJointeUrl = dto.PieceJointeUrl,
            NumeroJustificatif = dto.NumeroJustificatif,
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

    public static DepenseDto ToDto(Depense entity)
    {
        return new DepenseDto
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            Montant = entity.Montant,
            MotifDepense = entity.MotifDepense,
            Categorie = entity.Categorie,
            BeneficiaireNom = entity.BeneficiaireNom,
            BeneficiaireTelephone = entity.BeneficiaireTelephone,
            BeneficiaireNIF = entity.BeneficiaireNIF,
            StatutDepense = entity.StatutDepense,
            DateDemande = entity.DateDemande,
            DateValidation = entity.DateValidation,
            ValidateurId = entity.ValidateurId,
            PieceJointeUrl = entity.PieceJointeUrl,
            NumeroJustificatif = entity.NumeroJustificatif,
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
