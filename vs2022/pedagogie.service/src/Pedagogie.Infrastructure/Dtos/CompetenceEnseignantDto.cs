using System.ComponentModel.DataAnnotations.Schema;
using GnDapper.Entities;

namespace Pedagogie.Infrastructure.Dtos;

/// <summary>
/// DTO de mapping BDD pour la table pedagogie.competences_enseignant.
/// </summary>
[Table("pedagogie.competences_enseignant")]
public sealed class CompetenceEnseignantDto : IEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int MatiereId { get; set; }
    public string CycleEtude { get; set; } = string.Empty;

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }

    // ISoftDeletable
    public bool IsDeleted { get; set; }
}
