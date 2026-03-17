import type { BaseEntity } from '~/types/common'

export interface Signature extends BaseEntity {
  name: string
  description: string | null
  documentGenereId: number
  documentGenereNom?: string
  signataireId: number
  signataireNom?: string
  ordreSignature: number
  dateSignature: string | null
  statutSignature: string
  niveauSignature: string
  motifRefus: string | null
  estValidee: boolean
  typeId: number
  typeName: string
}

export interface CreateSignaturePayload {
  name: string
  description?: string | null
  documentGenereId: number
  signataireId: number
  ordreSignature: number
  statutSignature: string
  niveauSignature: string
  motifRefus?: string | null
  estValidee: boolean
  typeId: number
}

export interface UpdateSignaturePayload {
  id: number
  name?: string
  description?: string | null
  documentGenereId?: number
  signataireId?: number
  ordreSignature?: number
  dateSignature?: string | null
  statutSignature?: string
  niveauSignature?: string
  motifRefus?: string | null
  estValidee?: boolean
  typeId?: number
}

export interface SignatureFilters {
  search?: string
  typeId?: number
  statutSignature?: string
  niveauSignature?: string
  dateFrom?: string
  dateTo?: string
}

export interface SignatureType {
  id: number
  name: string
}

export const STATUTS_SIGNATURE = [
  'EnAttente',
  'Signe',
  'Refuse',
  'Delegue',
] as const

export const NIVEAUX_SIGNATURE = [
  'Basique',
  'Avancee',
] as const
