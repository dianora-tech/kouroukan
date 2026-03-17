import type { BaseEntity } from '~/types/common'

export interface DocumentGenere extends BaseEntity {
  name: string
  description: string | null
  modeleDocumentId: number
  modeleDocumentNom?: string
  eleveId: number | null
  eleveNom?: string
  enseignantId: number | null
  enseignantNom?: string
  donneesJson: string
  dateGeneration: string
  statutSignature: string
  cheminFichier: string | null
  typeId: number
  typeName: string
}

export interface CreateDocumentGenerePayload {
  name: string
  description?: string | null
  modeleDocumentId: number
  eleveId?: number | null
  enseignantId?: number | null
  donneesJson: string
  dateGeneration: string
  statutSignature: string
  cheminFichier?: string | null
  typeId: number
}

export interface UpdateDocumentGenerePayload {
  id: number
  name?: string
  description?: string | null
  modeleDocumentId?: number
  eleveId?: number | null
  enseignantId?: number | null
  donneesJson?: string
  statutSignature?: string
  cheminFichier?: string | null
  typeId?: number
}

export interface DocumentGenereFilters {
  search?: string
  typeId?: number
  statutSignature?: string
  dateFrom?: string
  dateTo?: string
}

export interface DocumentGenereType {
  id: number
  name: string
}

export const STATUTS_SIGNATURE_DOCUMENT = [
  'EnAttente',
  'EnCours',
  'Signe',
  'Refuse',
] as const

export const TYPES_DOCUMENT_GENERE = [
  'Bulletin',
  'Attestation',
  'Certificat',
  'Recu',
  'Contrat',
  'Convocation',
] as const
