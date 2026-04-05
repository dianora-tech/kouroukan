import type { BaseEntity } from '~/types/common'

// ── Competence ───────────────────────────────────────────
export interface Competence extends BaseEntity {
  matiere: string
  cycle: string
  userId: number
}

export interface CreateCompetencePayload {
  matiere: string
  cycle: string
}

// ── Liaison Enseignant ───────────────────────────────────
export interface LiaisonEnseignant extends BaseEntity {
  etablissement: string
  etablissementId: number
  statut: string
  date: string
  dateFin: string | null
}

// ── Affectation Enseignant ───────────────────────────────
export interface AffectationEnseignant extends BaseEntity {
  etablissement: string
  etablissementId: number
  classe: string
  classeId: number
  matiere: string
  matiereId: number
  jourSemaine: number
  heureDebut: string
  heureFin: string
  anneeScolaireId: number
}

// ── Heures d'enseignement (seances realisees) ───────────
export interface HeureEnseignant extends BaseEntity {
  date: string
  etablissement: string
  classe: string
  matiere: string
  duree: number
  statut: string
}

// ── Bulletin Enseignant ─────────────────────────────────
export interface BulletinEnseignant extends BaseEntity {
  etablissement: string
  classe: string
  trimestre: string
  date: string
  statut: string
  url: string
}

// ── Enseignant Filters ───────────────────────────────────
export interface EnseignantFilters {
  search?: string
  etablissementId?: number
  statut?: string
}
