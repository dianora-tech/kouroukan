import type { BaseEntity } from '~/types/common'

export interface Badgeage extends BaseEntity {
  eleveId: number
  eleveNom?: string
  dateBadgeage: string
  heureBadgeage: string
  pointAcces: string
  methodeBadgeage: string
  typeId: number
  typeName: string
}

export interface CreateBadgeagePayload {
  eleveId: number
  dateBadgeage: string
  heureBadgeage: string
  pointAcces: string
  methodeBadgeage: string
  typeId: number
}

export interface UpdateBadgeagePayload {
  id: number
  eleveId?: number
  dateBadgeage?: string
  heureBadgeage?: string
  pointAcces?: string
  methodeBadgeage?: string
  typeId?: number
}

export interface BadgeageFilters {
  search?: string
  typeId?: number
  eleveId?: number
  pointAcces?: string
  methodeBadgeage?: string
  dateFrom?: string
  dateTo?: string
}

export interface BadgeageType {
  id: number
  name: string
}

export const POINTS_ACCES = [
  'Entree',
  'Sortie',
  'Cantine',
  'Bibliotheque',
  'SalleDeSport',
] as const

export const METHODES_BADGEAGE = [
  'NFC',
  'QRCode',
  'Manuel',
] as const

export const TYPES_BADGEAGE = [
  'EntreeEtablissement',
  'SortieEtablissement',
  'Cantine',
  'Bibliotheque',
  'SalleDeSport',
] as const
