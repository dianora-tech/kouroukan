import type { BaseEntity } from '~/types/common'

// ── Liaison Parent ───────────────────────────────────────
export interface LiaisonParent extends BaseEntity {
  enfantId: number
  enfantNom: string
  enfantPrenom: string
  classe: string
  etablissement: string
  etablissementId: number
  matricule: string
  statut: string
}

// ── Journal Financier ────────────────────────────────────
export interface JournalFinancier extends BaseEntity {
  date: string
  montant: number
  description: string
  type: string
  methode: string
  statut: string
  enfantId: number
  enfantNom: string
}

export interface CreateJournalFinancierPayload {
  date: string
  montant: number
  description: string
  type: string
  methode: string
  enfantId: number
}

// ── Emploi du temps (seance) ────────────────────────────
export interface SeanceFamille {
  id: number
  jourSemaine: number
  heureDebut: string
  heureFin: string
  matiere: string
  salle: string
  enseignant: string
  classe: string
}

// ── Notes ────────────────────────────────────────────────
export interface NoteFamille extends BaseEntity {
  matiere: string
  note: number
  noteMax: number
  coefficient: number
  appreciation: string
  enseignant: string
  date: string
  type: string
  enfantId: number
}

// ── Communication (messages) ────────────────────────────
export interface MessageFamille extends BaseEntity {
  expediteur: string
  objet: string
  contenu: string
  date: string
  lu: boolean
  type: string
}

// ── Documents ────────────────────────────────────────────
export interface DocumentFamille extends BaseEntity {
  nom: string
  type: string
  date: string
  taille: string
  url: string
  enfantId: number
}

// ── Absences ─────────────────────────────────────────────
export interface AbsenceFamille extends BaseEntity {
  date: string
  motif: string
  justifie: boolean
  duree: string
  enfantId: number
  enfantNom: string
}

// ── Famille Filters ──────────────────────────────────────
export interface FamilleFilters {
  search?: string
  enfantId?: number
}
