namespace Kouroukan.Api.Gateway.Models;

// ─── Forfait User ─────────────────────────────────────────────────────────────

/// <summary>
/// Statut de l'abonnement actif d'un utilisateur ou etablissement.
/// </summary>
public class ForfaitStatusDto
{
    public int? AbonnementId { get; set; }
    public string? ForfaitNom { get; set; }
    public string? ForfaitCode { get; set; }
    public string? TypeCible { get; set; }
    public bool EstGratuit { get; set; }
    public int? LimiteEleves { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public DateTime? DateEssaiFin { get; set; }
    public bool EstActif { get; set; }
    public bool EstEnEssai { get; set; }
    public bool PeutResilier { get; set; }
    public int? NombreElevesActuel { get; set; }
}

/// <summary>
/// Plan forfait disponible a la souscription.
/// </summary>
public class ForfaitPlanDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PrixMensuel { get; set; }
    public int PrixVacances { get; set; }
    public int PeriodeEssaiJours { get; set; }
    public int? LimiteEleves { get; set; }
    public bool EstGratuit { get; set; }
}

/// <summary>
/// Historique d'un abonnement utilisateur.
/// </summary>
public class AbonnementHistoryDto
{
    public int Id { get; set; }
    public string ForfaitNom { get; set; } = string.Empty;
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public int Montant { get; set; }
    public string Statut { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Requete de souscription a un forfait.
/// </summary>
public class SubscribeForfaitRequest
{
    public int ForfaitId { get; set; }
    public int? EleveId { get; set; }
}

/// <summary>
/// Requete de resiliation d'un abonnement.
/// </summary>
public class CancelForfaitRequest
{
    public int AbonnementId { get; set; }
}

/// <summary>
/// Statistiques globales des forfaits (vue admin).
/// </summary>
public class ForfaitStatsDto
{
    public int TotalEtablissements { get; set; }
    public int EtablissementsAvecForfait { get; set; }
    public decimal TauxEtablissements { get; set; }
    public int TotalEnseignants { get; set; }
    public int EnseignantsAvecForfait { get; set; }
    public decimal TauxEnseignants { get; set; }
    public int TotalParents { get; set; }
    public int ParentsAvecForfait { get; set; }
    public decimal TauxParents { get; set; }
}

/// <summary>
/// Resultat de verification du quota d'eleves.
/// </summary>
public class QuotaCheckResult
{
    public bool LimiteAtteinte { get; set; }
    public int? Limite { get; set; }
    public int NombreActuel { get; set; }
    public string? Message { get; set; }
}
