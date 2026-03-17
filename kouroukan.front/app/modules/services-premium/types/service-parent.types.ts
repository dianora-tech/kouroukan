import type { BaseEntity } from '~/types/common'

export interface ServiceParent extends BaseEntity {
  code: string
  tarif: number
  periodicite: string
  estActif: boolean
  periodeEssaiJours: number | null
  tarifDegressif: boolean
  typeId: number
  typeName: string
}

export interface CreateServiceParentPayload {
  code: string
  tarif: number
  periodicite: string
  estActif: boolean
  periodeEssaiJours?: number | null
  tarifDegressif: boolean
  typeId: number
}

export interface UpdateServiceParentPayload {
  id: number
  code?: string
  tarif?: number
  periodicite?: string
  estActif?: boolean
  periodeEssaiJours?: number | null
  tarifDegressif?: boolean
  typeId?: number
}

export interface ServiceParentFilters {
  search?: string
  typeId?: number
  periodicite?: string
  estActif?: boolean
}

export interface ServiceParentType {
  id: number
  name: string
}

export const PERIODICITES = [
  'Mensuel',
  'Annuel',
  'Unite',
] as const
