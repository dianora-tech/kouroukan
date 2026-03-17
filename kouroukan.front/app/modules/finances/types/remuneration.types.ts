import type { BaseEntity } from '~/types/common'

export interface RemunerationEnseignant extends BaseEntity {
  enseignantId: number
  enseignantNom?: string
  mois: number
  annee: number
  modeRemuneration: string
  montantForfait: number | null
  nombreHeures: number | null
  tauxHoraire: number | null
  montantTotal: number
  statutPaiement: string
  dateValidation: string | null
  validateurId: number | null
  validateurNom?: string
}

export interface CreateRemunerationPayload {
  enseignantId: number
  mois: number
  annee: number
  modeRemuneration: string
  montantForfait?: number | null
  nombreHeures?: number | null
  tauxHoraire?: number | null
  montantTotal: number
  statutPaiement: string
}

export interface UpdateRemunerationPayload {
  id: number
  enseignantId?: number
  mois?: number
  annee?: number
  modeRemuneration?: string
  montantForfait?: number | null
  nombreHeures?: number | null
  tauxHoraire?: number | null
  montantTotal?: number
  statutPaiement?: string
}

export interface RemunerationFilters {
  search?: string
  mois?: number
  annee?: number
  modeRemuneration?: string
  statutPaiement?: string
}

export const STATUTS_REMUNERATION = [
  'EnAttente',
  'Valide',
  'Paye',
] as const

export const MODES_REMUNERATION = [
  'Forfait',
  'Heures',
  'Mixte',
] as const

export const MOIS_OPTIONS = [
  { value: 1, label: 'Janvier' },
  { value: 2, label: 'Février' },
  { value: 3, label: 'Mars' },
  { value: 4, label: 'Avril' },
  { value: 5, label: 'Mai' },
  { value: 6, label: 'Juin' },
  { value: 7, label: 'Juillet' },
  { value: 8, label: 'Août' },
  { value: 9, label: 'Septembre' },
  { value: 10, label: 'Octobre' },
  { value: 11, label: 'Novembre' },
  { value: 12, label: 'Décembre' },
] as const
