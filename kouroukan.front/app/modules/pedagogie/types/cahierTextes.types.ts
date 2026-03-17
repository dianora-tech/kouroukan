import type { BaseEntity } from '~/types/common'

export interface CahierTextes extends BaseEntity {
  seanceId: number
  seanceInfo?: string
  matiereName?: string
  classeName?: string
  enseignantNom?: string
  contenu: string
  dateSeance: string
  travailAFaire: string | null
}

export interface CreateCahierTextesPayload {
  seanceId: number
  contenu: string
  dateSeance: string
  travailAFaire?: string
}

export interface UpdateCahierTextesPayload {
  id: number
  seanceId?: number
  contenu?: string
  dateSeance?: string
  travailAFaire?: string | null
}

export interface CahierTextesFilters {
  search?: string
  seanceId?: number
  dateFrom?: string
  dateTo?: string
}
