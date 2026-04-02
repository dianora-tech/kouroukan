namespace Kouroukan.Api.Gateway.Models;

// ─── Pagination ────────────────────────────────────────────────────────────────

/// <summary>
/// Resultat pagine generique.
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

// ─── Forfaits ──────────────────────────────────────────────────────────────────

/// <summary>
/// DTO d'un forfait.
/// Colonnes reelles: id, code, nom, description, prix_mensuel, prix_vacances,
///   periode_essai_jours, type_cible, est_gratuit, limite_eleves, est_actif,
///   created_at, updated_at, created_by, is_deleted
/// </summary>
public class ForfaitDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PrixMensuel { get; set; }
    public int PrixVacances { get; set; }
    public int PeriodeEssaiJours { get; set; }
    public string TypeCible { get; set; } = string.Empty;
    public bool EstGratuit { get; set; }
    public int? LimiteEleves { get; set; }
    public bool EstActif { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Requete de creation d'un forfait.
/// </summary>
public class CreateForfaitRequest
{
    public string Code { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PrixMensuel { get; set; }
    public int PrixVacances { get; set; }
    public int PeriodeEssaiJours { get; set; } = 30;
    public string TypeCible { get; set; } = string.Empty;
    public bool EstGratuit { get; set; }
    public int? LimiteEleves { get; set; }
}

/// <summary>
/// Requete de mise a jour d'un forfait.
/// </summary>
public class UpdateForfaitRequest
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PrixMensuel { get; set; }
    public int PrixVacances { get; set; }
    public int PeriodeEssaiJours { get; set; }
    public string TypeCible { get; set; } = string.Empty;
    public bool EstGratuit { get; set; }
    public int? LimiteEleves { get; set; }
    public bool EstActif { get; set; }
}

/// <summary>
/// Requete de mise a jour du tarif d'un forfait.
/// Colonnes reelles forfait_tarifs: forfait_id, prix_mensuel, prix_vacances, date_effet
/// </summary>
public class UpdateForfaitTarifRequest
{
    public int PrixMensuel { get; set; }
    public int PrixVacances { get; set; }
    public DateTime DateEffet { get; set; }
}

// ─── Abonnements ───────────────────────────────────────────────────────────────

/// <summary>
/// DTO d'un abonnement.
/// Colonnes reelles: id, forfait_id, company_id, user_id, date_debut, date_fin,
///   date_essai_fin, est_actif, geste_commercial_id, created_at, updated_at,
///   created_by, is_deleted
/// </summary>
public class AbonnementDto
{
    public int Id { get; set; }
    public int ForfaitId { get; set; }
    public string ForfaitNom { get; set; } = string.Empty;
    public int? CompanyId { get; set; }
    public string? CompanyNom { get; set; }
    public int? UserId { get; set; }
    public string? UserNom { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public DateTime? DateEssaiFin { get; set; }
    public bool EstActif { get; set; }
    public int? GesteCommercialId { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Requete de creation d'un abonnement.
/// </summary>
public class CreateAbonnementRequest
{
    public int ForfaitId { get; set; }
    public int? CompanyId { get; set; }
    public int? UserId { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public DateTime? DateEssaiFin { get; set; }
    public int? GesteCommercialId { get; set; }
}

/// <summary>
/// Requete de mise a jour d'un abonnement.
/// </summary>
public class UpdateAbonnementRequest
{
    public DateTime? DateFin { get; set; }
    public DateTime? DateEssaiFin { get; set; }
    public bool EstActif { get; set; }
    public int? GesteCommercialId { get; set; }
}

// ─── Gestes Commerciaux ────────────────────────────────────────────────────────

/// <summary>
/// DTO d'un geste commercial.
/// Colonnes reelles: id, nom, description, type_cible, cible_valeur, forfait_id,
///   reduction_pourcent, reduction_montant, date_debut, date_fin, est_actif,
///   company_id, created_at, created_by, is_deleted
/// </summary>
public class GesteCommercialDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TypeCible { get; set; } = string.Empty;
    public string? CibleValeur { get; set; }
    public int? ForfaitId { get; set; }
    public string? ForfaitNom { get; set; }
    public int ReductionPourcent { get; set; }
    public int ReductionMontant { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public bool EstActif { get; set; }
    public int? CompanyId { get; set; }
    public string? CompanyNom { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Requete de creation d'un geste commercial.
/// </summary>
public class CreateGesteCommercialRequest
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TypeCible { get; set; } = string.Empty;
    public string? CibleValeur { get; set; }
    public int? ForfaitId { get; set; }
    public int ReductionPourcent { get; set; }
    public int ReductionMontant { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public int? CompanyId { get; set; }
}

/// <summary>
/// Requete de mise a jour d'un geste commercial.
/// </summary>
public class UpdateGesteCommercialRequest
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TypeCible { get; set; } = string.Empty;
    public string? CibleValeur { get; set; }
    public int? ForfaitId { get; set; }
    public int ReductionPourcent { get; set; }
    public int ReductionMontant { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public bool EstActif { get; set; }
}

// ─── Liaisons Enseignant ───────────────────────────────────────────────────────

/// <summary>
/// DTO d'une liaison enseignant.
/// </summary>
public class LiaisonEnseignantDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserNom { get; set; } = string.Empty;
    public int CompanyId { get; set; }
    public string CompanyNom { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public string? Identifiant { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? TerminatedAt { get; set; }
}

/// <summary>
/// Requete de creation d'une liaison enseignant (scan QR ou identifiant).
/// </summary>
public class CreateLiaisonEnseignantRequest
{
    public int CompanyId { get; set; }
    public string? Identifiant { get; set; }
    public string? QrCode { get; set; }
}

// ─── QR Codes ──────────────────────────────────────────────────────────────────

/// <summary>
/// DTO d'un QR code utilisateur.
/// </summary>
public class QrCodeDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// DTO de resolution d'un QR code vers un profil utilisateur.
/// </summary>
public class QrCodeResolvedDto
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Role { get; set; }
}

// ─── Email Config ──────────────────────────────────────────────────────────────

/// <summary>
/// DTO de configuration email.
/// Colonnes reelles: id, smtp_host, smtp_port, smtp_user, smtp_password,
///   email_expediteur, nom_expediteur, est_actif, created_at, updated_at
/// Pas de colonne is_deleted.
/// </summary>
public class EmailConfigDto
{
    public int Id { get; set; }
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SmtpUser { get; set; } = string.Empty;
    public string EmailExpediteur { get; set; } = string.Empty;
    public string NomExpediteur { get; set; } = string.Empty;
    public bool EstActif { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Requete de mise a jour de la configuration email.
/// </summary>
public class UpdateEmailConfigRequest
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SmtpUser { get; set; } = string.Empty;
    public string? SmtpPassword { get; set; }
    public string EmailExpediteur { get; set; } = string.Empty;
    public string NomExpediteur { get; set; } = string.Empty;
}

/// <summary>
/// Requete d'envoi d'un email de test.
/// </summary>
public class TestEmailRequest
{
    public string To { get; set; } = string.Empty;
}

// ─── SMS Config ────────────────────────────────────────────────────────────────

/// <summary>
/// DTO de configuration SMS (NimbaSMS).
/// Colonnes reelles: id, api_key, api_secret, sender_name, solde_actuel,
///   derniere_synchro, est_actif, created_at, updated_at
/// Pas de colonne is_deleted.
/// </summary>
public class SmsConfigDto
{
    public int Id { get; set; }
    public string ApiKey { get; set; } = string.Empty;
    public string? ApiSecret { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public bool EstActif { get; set; }
    public int? SoldeActuel { get; set; }
    public DateTime? DerniereSynchro { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Requete de mise a jour de la configuration SMS.
/// </summary>
public class UpdateSmsConfigRequest
{
    public string ApiKey { get; set; } = string.Empty;
    public string? ApiSecret { get; set; }
    public string SenderName { get; set; } = string.Empty;
}

// ─── Comptes Mobile Money ──────────────────────────────────────────────────────

/// <summary>
/// DTO d'un compte Mobile Money admin.
/// Colonnes reelles: id, operateur, numero_compte, code_marchand, libelle,
///   est_actif, created_at, updated_at, created_by, is_deleted
/// </summary>
public class CompteMobileDto
{
    public int Id { get; set; }
    public string Operateur { get; set; } = string.Empty;
    public string NumeroCompte { get; set; } = string.Empty;
    public string? CodeMarchand { get; set; }
    public string? Libelle { get; set; }
    public bool EstActif { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Requete de creation d'un compte Mobile Money.
/// </summary>
public class CreateCompteMobileRequest
{
    public string Operateur { get; set; } = string.Empty;
    public string NumeroCompte { get; set; } = string.Empty;
    public string? CodeMarchand { get; set; }
    public string? Libelle { get; set; }
}

/// <summary>
/// Requete de mise a jour d'un compte Mobile Money.
/// </summary>
public class UpdateCompteMobileRequest
{
    public string Operateur { get; set; } = string.Empty;
    public string NumeroCompte { get; set; } = string.Empty;
    public string? CodeMarchand { get; set; }
    public string? Libelle { get; set; }
    public bool EstActif { get; set; }
}

// ─── Contenu IA ────────────────────────────────────────────────────────────────

/// <summary>
/// DTO d'un contenu IA.
/// Colonnes reelles: id, rubrique, titre, contenu, est_actif, ordre,
///   created_at, updated_at, created_by, is_deleted
/// Pas de colonne 'langue'.
/// </summary>
public class ContenuIaDto
{
    public int Id { get; set; }
    public string Rubrique { get; set; } = string.Empty;
    public string Titre { get; set; } = string.Empty;
    public string Contenu { get; set; } = string.Empty;
    public bool EstActif { get; set; }
    public int Ordre { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Requete de creation d'un contenu IA.
/// </summary>
public class CreateContenuIaRequest
{
    public string Rubrique { get; set; } = string.Empty;
    public string Titre { get; set; } = string.Empty;
    public string Contenu { get; set; } = string.Empty;
    public int Ordre { get; set; }
}

/// <summary>
/// Requete de mise a jour d'un contenu IA.
/// </summary>
public class UpdateContenuIaRequest
{
    public string Rubrique { get; set; } = string.Empty;
    public string Titre { get; set; } = string.Empty;
    public string Contenu { get; set; } = string.Empty;
    public int Ordre { get; set; }
    public bool EstActif { get; set; }
}

// ─── Admin Etablissements ──────────────────────────────────────────────────────

/// <summary>
/// DTO d'un etablissement vu par l'admin.
/// </summary>
public class AdminEtablissementDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? RegionCode { get; set; }
    public string? PrefectureCode { get; set; }
    public string? SousPrefectureCode { get; set; }
    public string? RegionName { get; set; }
    public string? PrefectureName { get; set; }
    public string? SousPrefectureName { get; set; }
    public string[] Modules { get; set; } = [];
    public int UserCount { get; set; }
    public string? ForfaitNom { get; set; }
    public string? DirecteurNom { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO de detail d'un etablissement.
/// </summary>
public class AdminEtablissementDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? RegionCode { get; set; }
    public string? PrefectureCode { get; set; }
    public string? SousPrefectureCode { get; set; }
    public string? RegionName { get; set; }
    public string? PrefectureName { get; set; }
    public string? SousPrefectureName { get; set; }
    public string[] Modules { get; set; } = [];
    public int UserCount { get; set; }
    public string? ForfaitNom { get; set; }
    public string? DirecteurNom { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Requete de mise a jour d'un etablissement par l'admin.
/// </summary>
public class UpdateEtablissementRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? RegionCode { get; set; }
    public string? PrefectureCode { get; set; }
    public string? SousPrefectureCode { get; set; }
    public string[] Modules { get; set; } = [];
    public bool IsActive { get; set; }
}
