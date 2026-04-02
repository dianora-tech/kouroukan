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

// ── Famille Filters ──────────────────────────────────────
export interface FamilleFilters {
  search?: string
  enfantId?: number
}
