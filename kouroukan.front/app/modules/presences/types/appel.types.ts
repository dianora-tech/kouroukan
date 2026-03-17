import type { BaseEntity } from '~/types/common'

export interface Appel extends BaseEntity {
  classeId: number
  classeName?: string
  enseignantId: number
  enseignantNom?: string
  seanceId: number | null
  seanceInfo?: string
  dateAppel: string
  heureAppel: string
  estCloture: boolean
}

export interface CreateAppelPayload {
  classeId: number
  enseignantId: number
  seanceId?: number | null
  dateAppel: string
  heureAppel: string
  estCloture: boolean
}

export interface UpdateAppelPayload {
  id: number
  classeId?: number
  enseignantId?: number
  seanceId?: number | null
  dateAppel?: string
  heureAppel?: string
  estCloture?: boolean
}

export interface AppelFilters {
  search?: string
  classeId?: number
  enseignantId?: number
  estCloture?: boolean
  dateFrom?: string
  dateTo?: string
}
