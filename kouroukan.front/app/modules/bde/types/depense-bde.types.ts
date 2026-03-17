import type { BaseEntity } from '~/types/common'

export interface DepenseBDE extends BaseEntity {
  name: string
  description: string | null
  associationId: number
  associationNom?: string
  montant: number
  motif: string
  categorie: string
  statutValidation: string
  validateurId: number | null
  validateurNom?: string
  typeId: number
  typeName: string
}

export interface CreateDepenseBDEPayload {
  name: string
  description?: string
  associationId: number
  montant: number
  motif: string
  categorie: string
  statutValidation: string
  typeId: number
}

export interface UpdateDepenseBDEPayload {
  id: number
  name?: string
  description?: string
  associationId?: number
  montant?: number
  motif?: string
  categorie?: string
  statutValidation?: string
  typeId?: number
}

export interface DepenseBDEFilters {
  search?: string
  typeId?: number
  associationId?: number
  categorie?: string
  statutValidation?: string
}

export interface DepenseBDEType {
  id: number
  name: string
}

export const STATUTS_VALIDATION_BDE = [
  'Demandee',
  'ValideTresorier',
  'ValideSuper',
  'Refusee',
] as const

export const CATEGORIES_DEPENSE_BDE = [
  'Materiel',
  'Location',
  'Prestataire',
  'Remboursement',
] as const
