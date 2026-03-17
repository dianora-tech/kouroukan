import type { BaseEntity } from '~/types/common'

export interface SalleType {
  id: number
  name: string
}

export interface Salle extends BaseEntity {
  name: string
  capacite: number
  batiment: string | null
  equipements: string | null
  estDisponible: boolean
  typeId: number
  typeName?: string
}

export interface CreateSallePayload {
  name: string
  capacite: number
  batiment?: string
  equipements?: string
  estDisponible: boolean
  typeId: number
}

export interface UpdateSallePayload {
  id: number
  name?: string
  capacite?: number
  batiment?: string | null
  equipements?: string | null
  estDisponible?: boolean
  typeId?: number
}

export interface SalleFilters {
  search?: string
  typeId?: number
  estDisponible?: boolean
}
