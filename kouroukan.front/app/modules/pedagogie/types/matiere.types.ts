import type { BaseEntity } from '~/types/common'

export interface MatiereType {
  id: number
  name: string
}

export interface Matiere extends BaseEntity {
  name: string
  code: string
  typeId: number
  typeName?: string
}

export interface CreateMatierePayload {
  name: string
  code: string
  typeId: number
}

export interface UpdateMatierePayload {
  id: number
  name?: string
  code?: string
  typeId?: number
}

export interface MatiereFilters {
  search?: string
  typeId?: number
}
