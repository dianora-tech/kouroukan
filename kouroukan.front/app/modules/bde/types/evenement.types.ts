import type { BaseEntity } from '~/types/common'

export interface Evenement extends BaseEntity {
  name: string
  description: string | null
  associationId: number
  associationNom?: string
  dateEvenement: string
  lieu: string
  capacite: number | null
  tarifEntree: number | null
  nombreInscrits: number
  statutEvenement: string
  typeId: number
  typeName: string
}

export interface CreateEvenementPayload {
  name: string
  description?: string
  associationId: number
  dateEvenement: string
  lieu: string
  capacite?: number | null
  tarifEntree?: number | null
  nombreInscrits: number
  statutEvenement: string
  typeId: number
}

export interface UpdateEvenementPayload {
  id: number
  name?: string
  description?: string
  associationId?: number
  dateEvenement?: string
  lieu?: string
  capacite?: number | null
  tarifEntree?: number | null
  nombreInscrits?: number
  statutEvenement?: string
  typeId?: number
}

export interface EvenementFilters {
  search?: string
  typeId?: number
  associationId?: number
  statutEvenement?: string
  dateFrom?: string
  dateTo?: string
}

export interface EvenementType {
  id: number
  name: string
}

export const STATUTS_EVENEMENT = [
  'Planifie',
  'Valide',
  'EnCours',
  'Termine',
  'Annule',
] as const
