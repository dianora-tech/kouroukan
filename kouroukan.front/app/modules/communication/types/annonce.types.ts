import type { BaseEntity } from '~/types/common'

export interface Annonce extends BaseEntity {
  contenu: string
  dateDebut: string
  dateFin: string | null
  estActive: boolean
  cibleAudience: string
  priorite: number
  typeId: number
  typeName?: string
}

export interface CreateAnnoncePayload {
  contenu: string
  dateDebut: string
  dateFin?: string
  estActive: boolean
  cibleAudience: string
  priorite: number
  typeId: number
}

export interface UpdateAnnoncePayload {
  id: number
  contenu?: string
  dateDebut?: string
  dateFin?: string | null
  estActive?: boolean
  cibleAudience?: string
  priorite?: number
  typeId?: number
}

export interface AnnonceFilters {
  search?: string
  typeId?: number
  estActive?: boolean
  cibleAudience?: string
  priorite?: number
  dateFrom?: string
  dateTo?: string
}

export const CIBLES_AUDIENCE = [
  'Tous',
  'Parents',
  'Enseignants',
  'Eleves',
] as const

export const TYPES_ANNONCE = [
  'Information',
  'Urgente',
  'Evenement',
  'Administrative',
] as const

export const PRIORITES_ANNONCE = [1, 2, 3, 4, 5] as const
