import type { BaseEntity } from '~/types/common'

export interface Association extends BaseEntity {
  name: string
  description: string | null
  sigle: string | null
  anneeScolaire: string
  statut: string
  budgetAnnuel: number
  superviseurId: number | null
  superviseurNom?: string
  typeId: number
  typeName: string
}

export interface CreateAssociationPayload {
  name: string
  description?: string
  sigle?: string | null
  anneeScolaire: string
  statut: string
  budgetAnnuel: number
  superviseurId?: number | null
  typeId: number
}

export interface UpdateAssociationPayload {
  id: number
  name?: string
  description?: string
  sigle?: string | null
  anneeScolaire?: string
  statut?: string
  budgetAnnuel?: number
  superviseurId?: number | null
  typeId?: number
}

export interface AssociationFilters {
  search?: string
  typeId?: number
  statut?: string
  anneeScolaire?: string
}

export interface AssociationType {
  id: number
  name: string
}

export const STATUTS_ASSOCIATION = [
  'Active',
  'Suspendue',
  'Dissoute',
] as const
