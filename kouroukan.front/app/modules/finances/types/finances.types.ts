import type { BaseEntity } from '~/types/common'

// ── Moyen de Paiement ────────────────────────────────────
export interface MoyenPaiement extends BaseEntity {
  nom: string
  type: string
  description: string
  estActif: boolean
}

export interface CreateMoyenPaiementPayload {
  nom: string
  type: string
  description: string
}

export interface UpdateMoyenPaiementPayload {
  id: number
  nom?: string
  type?: string
  description?: string
  estActif?: boolean
}

// ── Palier Familial ──────────────────────────────────────
export interface PalierFamilial extends BaseEntity {
  nom: string
  nbEnfantsMin: number
  nbEnfantsMax: number
  reductionPourcentage: number
}

export interface CreatePalierFamilialPayload {
  nom: string
  nbEnfantsMin: number
  nbEnfantsMax: number
  reductionPourcentage: number
}

// ── Filters ──────────────────────────────────────────────
export interface FinancesFilters {
  search?: string
  type?: string
}
