import type { BaseEntity } from '~/types/common'

export interface MatiereType {
  id: number
  name: string
}

export interface Matiere extends BaseEntity {
  name: string
  code: string
  coefficient: number
  nombreHeures: number
  niveauClasseId: number
  niveauClasseName?: string
  niveauClasseCode?: string
  typeId: number
  typeName?: string
}

export interface CreateMatierePayload {
  name: string
  code: string
  coefficient: number
  nombreHeures: number
  niveauClasseId: number
  typeId: number
}

export interface UpdateMatierePayload {
  id: number
  name?: string
  code?: string
  coefficient?: number
  nombreHeures?: number
  niveauClasseId?: number
  typeId?: number
}

export interface MatiereFilters {
  search?: string
  typeId?: number
  niveauClasseId?: number
}
